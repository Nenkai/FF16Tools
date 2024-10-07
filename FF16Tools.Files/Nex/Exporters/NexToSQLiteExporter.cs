using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData;

using FF16Tools.Files.Nex.Managers;
using FF16Tools.Files.Nex.Entities;
using FF16Tools.Files;
using System.IO;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;

namespace FF16Tools.Files.Nex.Exporters;

/// <summary>
/// Game database to sqlite exporter (disposable object).
/// </summary>
public class NexToSQLiteExporter : IDisposable
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private NexDatabase _database;
    private SqliteConnection _con;

    // We don't want byte arrays to be converted to base64.
    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonByteArrayConverter() } };

    public NexToSQLiteExporter(NexDatabase database, ILoggerFactory loggerFactory = null)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        ArgumentNullException.ThrowIfNull(database, nameof(database));

        _database = database;

        if (loggerFactory is not null)
            _logger = loggerFactory.CreateLogger(GetType().ToString());
    }

    public void Dispose()
    {
        _con.Dispose();
    }

    /// <summary>
    /// Exports the database to the specified sqlite file (will be created if it does not exist). The sqlite connection will be opened and closed.
    /// </summary>
    /// <param name="sqliteDbFile"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public void ExportTables(string sqliteDbFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqliteDbFile, nameof(sqliteDbFile));

        _con = new SqliteConnection($"Data Source={sqliteDbFile}");
        _con.Open();

        foreach (var table in _database.Tables)
        {
            _logger?.LogInformation("Exporting table '{tableName}'", table.Key);
            if (!TableMappingReader.LayoutExists(table.Key))
            {
                _logger?.LogWarning("Skipping table '{tableName}', no layout file", table.Key);
                continue;
            }

            List<NexRowInfo> nexRows = table.Value.RowManager.GetAllRowInfos();

            NexTableLayout tableColumnLayout = TableMappingReader.ReadTableLayout(table.Key, new Version(1, 0, 0));
            ExportTableToSQLite(table.Key, table.Value, tableColumnLayout, nexRows);
        }

        _con.Close();
    }


    private void ExportTableToSQLite(string tableName, NexDataFile nexFile, NexTableLayout columnLayout, List<NexRowInfo> rows)
    {
        //SQL: DROP TABLE IF EXISTS
        var command = _con.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS \"{tableName}\";";

        _logger?.LogTrace("Running DROP TABLE IF EXISTS for '{tableName}'.", tableName);
        command.ExecuteNonQuery();

        //SQL: CREATE TABLE
        string tableDefinition = $"CREATE TABLE {tableName} (";

        switch (nexFile.Type)
        {
            case NexTableType.SingleKeyed:
                tableDefinition += "Key INTEGER";
                break;

            case NexTableType.DoubleKeyed:
                tableDefinition += "Key INTEGER, Key2 INTEGER";
                break;

            case NexTableType.TripleKeyed:
                tableDefinition += "Key INTEGER, Key2 INTEGER, Key3 INTEGER";
                break;
        }

        if (columnLayout.Columns.Count > 0)
            tableDefinition += ", ";

        foreach (NexStructColumn column in columnLayout.Columns)
        {
            tableDefinition += $"\"{column.Name}\" {NexUtils.TypeToSQLiteTypeIdentifier(column.Type)}, ";
        }


        tableDefinition = tableDefinition.Remove(tableDefinition.Length - 2); // replace trailing comma
        tableDefinition += ");";

        command = _con.CreateCommand();
        command.CommandText = tableDefinition;

        _logger?.LogTrace("Running CREATE TABLE for '{tableName}'.", tableName);
        command.ExecuteNonQuery();

        //SQL: INSERT INTO
        if (rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            CreateInsertInto(sb, tableName, nexFile, columnLayout);

            for (int entryCounter = 0; entryCounter < rows.Count; entryCounter++)
            {
                sb.Append(" (");
                var row = rows[entryCounter];

                switch (nexFile.Type)
                {
                    case NexTableType.SingleKeyed:
                        sb.Append($"{row.Key}");
                        break;

                    case NexTableType.DoubleKeyed:
                        sb.Append($"{row.Key}, {row.Key2}");
                        break;

                    case NexTableType.TripleKeyed:
                        sb.Append($"{row.Key}, {row.Key2}, {row.Key3}");
                        break;
                }

                if (columnLayout.Columns.Count > 0)
                    sb.Append(", ");

                var cells = NexUtils.ReadRow(columnLayout, nexFile.Buffer, row.RowDataOffset);
                for (int i = 0; i < columnLayout.Columns.Count; i++)
                {
                    NexStructColumn column = columnLayout.Columns[i];

                    if (column.Type == NexColumnType.CustomStructArray)
                        sb.Append('\'');

                    string cell = CellToSql(tableName, column.Name, cells[i], columnLayout, column);
                    sb.Append(cell);

                    if (column.Type == NexColumnType.CustomStructArray)
                        sb.Append('\'');

                    if (i < columnLayout.Columns.Count - 1)
                        sb.Append(", ");
                }

                sb.Append(')');
                if (entryCounter < rows.Count - 1)
                    sb.Append(",\n");

                // Run an early insert (batched queries)
                if (entryCounter % 100 == 99 && entryCounter < rows.Count - 25)
                {
                    sb.Remove(sb.Length - 2, 2); // replace , with ;
                    sb.Append(';');

                    command = _con.CreateCommand();
                    command.CommandText = sb.ToString();

                    _logger?.LogTrace("Running early ({percent}%) INSERT INTO for '{tableName}'.", 100.0f * entryCounter / (1.0f * rows.Count), tableName);
                    command.ExecuteNonQuery();
                    sb.Clear();

                    // Start over
                    CreateInsertInto(sb, tableName, nexFile, columnLayout);
                }
            }

            sb.Append(";\n");

            command = _con.CreateCommand();
            command.CommandText = sb.ToString();

            _logger?.LogTrace("Running INSERT INTO for '{tableName}'.", tableName);
            command.ExecuteNonQuery();
        }
    }

    private static void CreateInsertInto(StringBuilder sb, string tableName, NexDataFile nexFile, NexTableLayout columnLayout)
    {
        sb.Append($"INSERT INTO \"{tableName}\"\n\t(");
        switch (nexFile.Type)
        {
            case NexTableType.SingleKeyed:
                sb.Append("Key");
                break;

            case NexTableType.DoubleKeyed:
                sb.Append("Key, Key2");
                break;

            case NexTableType.TripleKeyed:
                sb.Append("Key, Key2, Key3");
                break;
        }

        if (columnLayout.Columns.Count > 0)
            sb.Append(", ");

        for (int i = 0; i < columnLayout.Columns.Count; i++)
        {
            NexStructColumn column = columnLayout.Columns[i];
            sb.Append($"\"{column.Name}\"");
            if (i < columnLayout.Columns.Count - 1)
                sb.Append(", ");
        }

        sb.Append(")\nVALUES\n");
    }

    private static string CellToSql(string tableName, string columnName, object cell, NexTableLayout tableColumnLayout, NexStructColumn column)
    {
        if (column.Type == NexColumnType.CustomStructArray)
        {
            object[] structArray = (object[])cell;

            return JsonSerializer.Serialize(structArray, _jsonSerializerOptions);
        }
        else
        {
            return column.Type switch
            {
                NexColumnType.String => string.IsNullOrEmpty((string)cell) ? "NULL" : $"\'{((string)cell).Replace("\'", "\'\'")}\'",
                NexColumnType.Int => $"{(int)cell}",
                NexColumnType.UInt => $"{(uint)cell}",
                NexColumnType.Float => $"{(float)cell}",
                NexColumnType.Int64 => $"{(ulong)cell}",
                NexColumnType.Short => $"{(short)cell}",
                NexColumnType.UShort => $"{(ushort)cell}",
                NexColumnType.Byte => $"{(byte)cell}",
                NexColumnType.SByte => $"{(sbyte)cell}",
                NexColumnType.Double => $"{(double)cell}",
                NexColumnType.ByteArray => $"\"{JsonSerializer.Serialize((byte[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.IntArray => $"\"{JsonSerializer.Serialize((int[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.FloatArray => $"\"{JsonSerializer.Serialize((float[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.StringArray => $"\'{JsonSerializer.Serialize((string[])cell, _jsonSerializerOptions).Replace("\'", "\'\'")}\'",
                _ => throw new InvalidDataException($"Unexpected type '{column.Type}' for column '{columnName}' in table '{tableName}'")
            };
        }
    }
}