using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Data.Sqlite;

using FF16Tools.Files.Nex;
using FF16Tools.Files.Nex.Entities;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;

namespace FF16Tools.Nex.Sqlite;

/// <summary>
/// Game database to sqlite exporter (disposable object).
/// </summary>
public class NexToSQLiteExporter : IDisposable
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger? _logger;

    private NexDatabase _database;
    private SqliteConnection? _con;
    private Version _version;
    private string _codeName;

    // We don't want byte arrays to be converted to base64.
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    { 
        IncludeFields = true, // Required for unions.
        Converters = 
        { 
            new JsonByteArrayConverter() 
        } 
    };

    /// <summary>
    /// Nex to SQLite exporter.
    /// </summary>
    /// <param name="database">Nex database. (You can get one with <see cref="NexDatabase.Open(string, ILoggerFactory)>"/>)</param>
    /// <param name="version">Version. Should match game version and be at least 1.0.0.</param>
    /// <param name="loggerFactory">Logger factory, for logging.</param>
    public NexToSQLiteExporter(NexDatabase database, Version version, string codeName, ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(database, nameof(database));
        ArgumentNullException.ThrowIfNull(version, nameof(version));
        ArgumentNullException.ThrowIfNull(codeName, nameof(codeName));

        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        _database = database;
        _version = version;
        _codeName = codeName;

        _loggerFactory = loggerFactory;
        if (loggerFactory is not null)
            _logger = loggerFactory.CreateLogger(GetType().ToString());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _con?.Dispose();
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

        // That'll improve performance by not creating the journal file everytime
        var com = _con.CreateCommand();
        com.CommandText = "PRAGMA journal_mode = MEMORY;";
        com.ExecuteNonQuery();

        CreateUnionTable();

        foreach (var table in _database.Tables)
        {
            _logger?.LogInformation("Exporting table '{tableName}'", table.Key);
            if (!TableMappingReader.LayoutExists(table.Key, _codeName))
            {
                _logger?.LogWarning("Skipping table '{tableName}', no layout file", table.Key);
                continue;
            }

            string tableNameWithoutLocale = table.Key;
            string locale = string.Empty;
            string[] spl = table.Key.Split('.');
            if (spl.Length >= 2)
            {
                tableNameWithoutLocale = spl[0];
                locale = spl[1];
            }

            List<NexRowInfo> nexRows = table.Value.RowManager!.GetAllRowInfos();

            NexTableLayout tableColumnLayout = TableMappingReader.ReadTableLayout(tableNameWithoutLocale, new Version(1, 0, 0), _codeName);
            string exportTableName = tableColumnLayout.Name + (!string.IsNullOrEmpty(locale) ? $".{locale}" : string.Empty);
            ExportTableToSQLite(exportTableName, table.Value, tableColumnLayout, nexRows);
        }

        _con.Close();
    }


    private void CreateUnionTable()
    {
        _logger?.LogInformation("Creating union table...");

        var command = _con!.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS _uniontypes;";
        _logger?.LogTrace("Running DROP TABLE IF EXISTS for '_uniontypes'.");
        command.ExecuteNonQuery();

        command = _con.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS _uniontypes (id INTEGER PRIMARY KEY, name TEXT)";
        _logger?.LogTrace("{command}", command.CommandText);
        command.ExecuteNonQuery();

        command = _con.CreateCommand();
        command.CommandText = $"INSERT INTO _uniontypes (Id, Name) VALUES (-100, '(FF16Tools) Do not edit/delete this table unless you know what you are doing.')";
        command.ExecuteNonQuery();

        foreach (var kv in NexUnions.UnionTypes[_codeName])
        {
            command = _con.CreateCommand();
            command.CommandText = $"INSERT INTO _uniontypes (Id, Name) VALUES ({(ushort)kv.Key},'{kv.Value}')";
            _logger?.LogTrace("{command}", command.CommandText);

            command.ExecuteNonQuery();
        }
    }


    private void ExportTableToSQLite(string tableName, NexDataFile nexFile, NexTableLayout columnLayout, List<NexRowInfo> rows)
    {
        tableName = tableName.Replace('.', '-');

        //SQL: DROP TABLE IF EXISTS
        var command = _con!.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS \"{tableName}\";";
        _logger?.LogTrace("Running DROP TABLE IF EXISTS for '{tableName}'.", tableName);
        command.ExecuteNonQuery();

        //SQL: CREATE TABLE
        string tableDefinition = $"CREATE TABLE \"{tableName}\" (";

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

        foreach (NexStructColumn column in columnLayout.Columns.Values)
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

                var cells = NexUtils.ReadRow(columnLayout, nexFile.Buffer!, row.RowDataOffset);

                int i = 0;
                foreach (NexStructColumn column in columnLayout.Columns.Values)
                {
                    if (column.Type == NexColumnType.CustomStructArray)
                        sb.Append('\'');

                    object cell = cells[i];
                    if (column.Type == NexColumnType.Float && cell is float f && float.IsNaN(f))
                    {
                        _logger?.LogWarning("Row [{k1},{k2},{k3}] has invalid float at {column}, defaulting to 0!", row.Key, row.Key2, row.Key3, column.Name);
                        cell = 0.0f;
                    }
                    else if (column.Type == NexColumnType.FloatArray)
                    {
                        float[] arr = (float[])cell;
                        for (int j = 0; j < arr.Length; j++)
                        {
                            if (float.IsNaN(arr[j]))
                            {
                                _logger?.LogWarning("Row [{k1},{k2},{k3}] has invalid float at {column} array index {index}, defaulting to 0!", row.Key, row.Key2, row.Key3, j, column.Name);
                                arr[j] = 0.0f;
                            }
                        }
                    }

                    if (column.Name == "Comment" && columnLayout.RowComments.TryGetValue((row.Key, row.Key2, row.Key3), out string? val))
                        cell = $"(FF16Tools): {val}";

                    string cellStr = CellToSql(tableName, column.Name, cell, columnLayout, column);
                    sb.Append(cellStr);

                    if (column.Type == NexColumnType.CustomStructArray)
                        sb.Append('\'');

                    if (i < columnLayout.Columns.Count - 1)
                        sb.Append(", ");

                    i++;
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

        int i = 0;
        foreach (NexStructColumn column in columnLayout.Columns.Values)
        {
            sb.Append($"\"{column.Name}\"");
            if (i < columnLayout.Columns.Count - 1)
                sb.Append(", ");

            i++;
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
                NexColumnType.HexUInt => $"'{(uint)cell:X8}'",
                NexColumnType.Float => $"{(float)cell}",
                NexColumnType.Int64 => $"{(ulong)cell}",
                NexColumnType.Short => $"{(short)cell}",
                NexColumnType.UShort => $"{(ushort)cell}",
                NexColumnType.Byte => $"{(byte)cell}",
                NexColumnType.SByte => $"{(sbyte)cell}",
                NexColumnType.Double => $"{(double)cell}",
                NexColumnType.NexUnionKey32 or NexColumnType.NexUnionKey16 => $"\"{((NexUnionKey)cell).GetNameOrValue()}:{((NexUnionKey)cell).Value}\"",
                NexColumnType.NexUnionKey32Array => $"\"[{string.Join(',', ((NexUnionKey[])cell).Select(e => $"{e.Type}:{e.Value}"))}]\"",
                NexColumnType.ByteArray => $"\"{JsonSerializer.Serialize((byte[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.ShortArray => $"\"{JsonSerializer.Serialize((short[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.IntArray => $"\"{JsonSerializer.Serialize((int[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.UIntArray => $"\"{JsonSerializer.Serialize((uint[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.FloatArray => $"\"{JsonSerializer.Serialize((float[])cell, _jsonSerializerOptions)}\"",
                NexColumnType.StringArray => $"\'{JsonSerializer.Serialize((string[])cell, _jsonSerializerOptions).Replace("\'", "\'\'")}\'",
                _ => throw new InvalidDataException($"Unexpected type '{column.Type}' for column '{columnName}' in table '{tableName}'")
            };
        }
    }
}