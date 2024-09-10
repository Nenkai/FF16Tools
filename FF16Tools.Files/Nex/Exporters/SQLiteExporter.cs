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
using FF16Tools.Files.Nex;
using FF16Tools.Files;
using System.IO;
using System.Data;
using Microsoft.Extensions.Logging;

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

    public NexToSQLiteExporter(NexDatabase database, ILoggerFactory loggerFactory = null)
    {
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

            NexTableColumnLayout tableColumnLayout = TableMappingReader.ReadColumnMappings(table.Key, new Version(1, 0, 0));
            ExportTableToSQLite(table.Key, table.Value, tableColumnLayout, nexRows);
        }

        _con.Close();
    }


    private void ExportTableToSQLite(string tableName, NexDataFile nexFile, NexTableColumnLayout columnLayout, List<NexRowInfo> rows)
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
            case NexTableType.Rows:
                tableDefinition += "RowID INTEGER";
                break;

            case NexTableType.RowSets:
                tableDefinition += "RowID INTEGER, ArrayIndex INTEGER";
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
                    case NexTableType.Rows:
                        sb.Append($"{row.Id}");
                        break;

                    case NexTableType.RowSets:
                        sb.Append($"{row.Id}, {row.ArrayIndex}");
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

    private static void CreateInsertInto(StringBuilder sb, string tableName, NexDataFile nexFile, NexTableColumnLayout columnLayout)
    {
        sb.Append($"INSERT INTO \"{tableName}\"\n\t(");
        switch (nexFile.Type)
        {
            case NexTableType.Rows:
                sb.Append("RowId");
                break;

            case NexTableType.RowSets:
                sb.Append("RowId, ArrayIndex");
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

    private static string CellToSql(string tableName, string columnName, object cell, NexTableColumnLayout tableColumnLayout, NexStructColumn column)
    {
        if (column.Type == NexColumnType.CustomStructArray)
        {
            StringBuilder sb = new StringBuilder();
            object[] structArray = (object[])cell;
            for (int i = 0; i < structArray.Length; i++)
            {
                sb.Append('(');
                object[] fields = (object[])structArray[i];
                for (int j = 0; j < fields.Length; j++)
                {
                    if (!string.IsNullOrEmpty(column.StructTypeName))
                    {
                        sb.Append('(');
                        var structFields = tableColumnLayout.CustomStructDefinitions[column.StructTypeName];
                        for (int k = 0; k < structFields.Count; k++)
                        {
                            sb.Append(CellToSql(tableName, column.Name, fields[k], tableColumnLayout, structFields[k]));
                            if (k != structFields.Count - 1)
                                sb.Append(", ");
                        }
                        sb.Append(')');
                    }
                    else
                        throw new Exception("Struct type name was empty");

                    if (j != fields.Length - 1)
                        sb.Append(", ");
                }
                sb.Append(')');
                if (i != structArray.Length - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
        else
        {
            return column.Type switch
            {
                NexColumnType.String => $"\"{((string)cell)}\"",
                NexColumnType.Int => $"{(int)cell}",
                NexColumnType.UInt => $"{(uint)cell}",
                NexColumnType.Float => $"{(float)cell}",
                NexColumnType.Int64 => $"{(ulong)cell}",
                NexColumnType.Short => $"{(short)cell}",
                NexColumnType.UShort => $"{(ushort)cell}",
                NexColumnType.Byte => $"{(byte)cell}",
                NexColumnType.SByte => $"{(sbyte)cell}",
                NexColumnType.Double => $"{(double)cell}",
                NexColumnType.ByteArray => $"\"{string.Join(", ", (byte[])cell)}\"",
                NexColumnType.IntArray => $"\"{string.Join(", ", (int[])cell)}\"",
                NexColumnType.FloatArray => $"\"{string.Join(", ", (float[])cell)}\"",
                NexColumnType.StringArray => $"\"{string.Join(", ", ((string[])cell).Select(e => e.Replace("'", "\'\'").Replace("\"", "\"\"")))}\"",
                _ => throw new InvalidDataException($"Unexpected type '{column.Type}' for column '{columnName}' in table '{tableName}'")
            };
        }
    }
}