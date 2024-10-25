using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using System.Text.Json;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using FF16Tools.Files.Nex.Entities;

namespace FF16Tools.Files.Nex.Exporters;

/// <summary>
/// Game database to sqlite exporter (disposable object).
/// </summary>
public class SQLiteToNexImporter : IDisposable
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private Dictionary<string, NexDataFileBuilder> _tableBuilders = [];
    private Dictionary<string, NexTableLayout> _tableLayouts = [];

    private List<string> _tablesToConvert;
    private string _sqliteFile;
    private SqliteConnection _con;

    // We don't want byte arrays to be converted to base64.
    private static JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new JsonByteArrayConverter() } };

    public SQLiteToNexImporter(string sqliteFile, List<string> tablesToConvert = null, ILoggerFactory loggerFactory = null)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        ArgumentException.ThrowIfNullOrWhiteSpace(sqliteFile, nameof(sqliteFile));

        _tablesToConvert = tablesToConvert;


        _sqliteFile = sqliteFile;

        if (loggerFactory is not null)
            _logger = loggerFactory.CreateLogger(GetType().ToString());
    }

    public void ReadSqlite()
    {
        _con = new SqliteConnection($"Data Source={_sqliteFile}");
        _con.Open();

        CreateTables();
        FillTables();

        _con.Close();
    }

    public void SaveTo(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory, nameof(directory));

        Directory.CreateDirectory(directory);

        foreach (var table in _tableBuilders)
        {
            string nxdPath = Path.Combine(directory, $"{table.Key}.nxd");

            _logger?.LogInformation("Writing {file}", nxdPath);
            using var fs = new FileStream(nxdPath, FileMode.Create);
            table.Value.Write(fs);
        }

        _logger?.LogInformation("Exported to {directory}.", directory);
    }

    private void CreateTables()
    {
        var command = _con.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';";
        _logger?.LogTrace(command.CommandText);

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            string tableName = (string)reader["name"];
            if (!TableMappingReader.LayoutExists(tableName))
            {
                _logger?.LogInformation("Skipping table {tableName}, no layout exists", tableName);
                continue;
            }

            if (_tablesToConvert.Count != 0 && !_tablesToConvert.Contains(tableName))
                continue;

            _logger?.LogInformation("Fetching table {tableName}", tableName);

            var layout = TableMappingReader.ReadTableLayout(tableName, new Version(1, 0, 0));
            _tableBuilders.Add(tableName, new NexDataFileBuilder(layout, _loggerFactory));
            _tableLayouts.Add(tableName, layout);
        }

        _logger?.LogInformation("Loaded {count} tables from SQLite", _tableLayouts.Count);
    }

    private void FillTables()
    {
        foreach (var table in _tableBuilders)
        {
            var command = _con.CreateCommand();
            command.CommandText = $"SELECT * FROM {table.Key};";
            _logger.LogTrace(command.CommandText);

            var reader = command.ExecuteReader();

            _logger?.LogInformation("Reading from {tableName}...", table.Key);

            NexDataFileBuilder tableBuilder = _tableBuilders[table.Key];
            NexTableLayout tableLayout = _tableLayouts[table.Key];

            var rows = reader.GetSchemaTable().Rows.Cast<DataRow>();

            foreach (NexStructColumn column in tableLayout.Columns.Values)
            {
                if (!rows.Any(e => (string)e["ColumnName"] == column.Name))
                {
                    throw new InvalidDataException($"Column '{column.Name}' for table '{table.Key}' does not exist in sqlite table");
                }
            }

            NexStructColumn lastColumn = null;
            uint key = 0, key2 = 0, key3 = 0;

            try
            {
                while (reader.Read())
                {
                    key = 0;
                    key2 = 0;
                    key3 = 0;

                    List<object> cells = [];
                    if (tableLayout.Type == NexTableType.SingleKeyed)
                    {
                        key = (uint)(long)reader["Key"];
                    }
                    else if (tableLayout.Type == NexTableType.DoubleKeyed)
                    {
                        key = (uint)(long)reader["Key"];
                        key2 = (uint)(long)reader["Key2"];
                    }
                    else if (tableLayout.Type == NexTableType.TripleKeyed)
                    {
                        key = (uint)(long)reader["Key"];
                        key2 = (uint)(long)reader["Key2"];
                        key3 = (uint)(long)reader["Key3"];
                    }
                    else
                        throw new NotImplementedException($"Table layout type {tableLayout.Type} not yet supported");

                    foreach (NexStructColumn column in tableLayout.Columns.Values)
                    {
                        lastColumn = column;
                        object val = reader[lastColumn.Name];

                        object cell = ParseCell(tableLayout, lastColumn, val);
                        cells.Add(cell);
                    }

                    tableBuilder.AddRow(key, key2, key3, cells);
                    key++;
                }
            }
            catch (Exception ex)
            {
                string message = $"Error in row (Key: {key}, Key2: {key2}, Key3: {key3}";
                if (lastColumn is not null)
                    message += $", At Column: {lastColumn.Name} ({lastColumn.Type})";
                message += ")\n";

                throw new Exception(message + ex.Message);
            }
        }
    }

    private object ParseCell(NexTableLayout tableLayout, NexStructColumn column, object val)
    {
        switch (column.Type)
        {
            case NexColumnType.SByte:
                return val is not DBNull ? (sbyte)(long)val : (sbyte)0;
            case NexColumnType.Byte:
                return val is not DBNull ? (byte)(long)val : (byte)0;
            case NexColumnType.Short:
                return val is not DBNull ? (short)(long)val : (short)0;
            case NexColumnType.UShort:
                return val is not DBNull ? (ushort)(long)val : (ushort)0;
            case NexColumnType.Int:
                return val is not DBNull ? (int)(long)val : 0;
            case NexColumnType.UInt:
                return val is not DBNull ? (uint)(long)val : 0u;
            case NexColumnType.HexUInt:
                {
                    var hexStr = val is not DBNull ? (string)val : "";
                    if (!NexUtils.TryParseHexUint(hexStr, out uint value))
                        throw new Exception($"Unable to parse hexadecimal uint {column.Name}");

                    return value;
                }
            case NexColumnType.Float:
                return val is not DBNull ? (float)(double)val : 0f;
            case NexColumnType.Int64:
                return val is not DBNull ? (long)val : 0L;
            case NexColumnType.Double:
                return val is not DBNull ? (double)val : 0d;
            case NexColumnType.String:
                return val is not DBNull ? (string)val : string.Empty;
            case NexColumnType.ByteArray:
                {
                    if (val is DBNull)
                        return Array.Empty<byte>();

                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        byte[] arr = JsonSerializer.Deserialize<byte[]>(arrStr, _jsonSerializerOptions);
                        return arr;
                    }
                    else
                        return Array.Empty<byte>();
                }
            case NexColumnType.IntArray:
                {
                    if (val is DBNull)
                        return Array.Empty<int>();

                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        int[] arr = JsonSerializer.Deserialize<int[]>(arrStr);
                        return arr;
                    }
                    else
                        return Array.Empty<int>();
                }
            case NexColumnType.UIntArray:
                {
                    if (val is DBNull)
                        return Array.Empty<uint>();

                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        uint[] arr = JsonSerializer.Deserialize<uint[]>(arrStr);
                        return arr;
                    }
                    else
                        return Array.Empty<uint>();
                }
            case NexColumnType.FloatArray:
                {
                    if (val is DBNull)
                        return Array.Empty<float>();

                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        float[] arr = JsonSerializer.Deserialize<float[]>(arrStr);
                        return arr;
                    }
                    else
                        return Array.Empty<float>();
                }
            case NexColumnType.StringArray:
                {
                    if (val is DBNull)
                        return Array.Empty<string>();

                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        string[] arr = JsonSerializer.Deserialize<string[]>(arrStr);
                        return arr;
                    }
                    else
                        return Array.Empty<string>();
                }
            case NexColumnType.CustomStructArray:
                {
                    if (val is DBNull)
                        return Array.Empty<object>();

                    string arrStr = (string)val;
                    var col = tableLayout.CustomStructDefinitions[column.StructTypeName];

                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        JsonElement obj = (JsonElement)JsonSerializer.Deserialize<object>(arrStr);
                        object[] array = new object[obj.GetArrayLength()];

                        int arrayIndex = 0;
                        foreach (JsonElement item in obj.EnumerateArray())
                        {
                            var structItem = new object[col.Count];
                            int itemsInJson = item.GetArrayLength();
                            if (itemsInJson != col.Count)
                                throw new Exception($"Error in column {column.Name} - expected {col.Count} fields in struct array index {arrayIndex}, got {itemsInJson} in json.");

                            int fieldIndex = 0;
                            foreach (JsonElement field in item.EnumerateArray())
                            {
                                switch (col[fieldIndex].Type)
                                {
                                    case NexColumnType.Short:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, col[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetInt16();
                                        break;
                                    case NexColumnType.String:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.String, col[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetString();
                                        break;
                                    case NexColumnType.Float:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, col[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetSingle();
                                        break;
                                    case NexColumnType.Int:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, col[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetInt32();
                                        break;
                                    case NexColumnType.UInt:
                                    case NexColumnType.HexUInt:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, col[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetUInt32();
                                        break;
                                    case NexColumnType.ByteArray:
                                        {
                                            ThrowIfStructElemNotValueKind(field, JsonValueKind.Array, col[fieldIndex], arrayIndex, fieldIndex);
                                            int arrLen = field.GetArrayLength();
                                            var arr = new byte[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetByte();
                                            structItem[fieldIndex] = arr;
                                            break;
                                        }
                                    case NexColumnType.IntArray:
                                        {
                                            ThrowIfStructElemNotValueKind(field, JsonValueKind.Array, col[fieldIndex], arrayIndex, fieldIndex);

                                            int arrLen = field.GetArrayLength();
                                            var arr = new int[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetInt32();
                                            structItem[fieldIndex] = arr;
                                            break;
                                        }
                                    case NexColumnType.UIntArray:
                                        {
                                            ThrowIfStructElemNotValueKind(field, JsonValueKind.Array, col[fieldIndex], arrayIndex, fieldIndex);

                                            int arrLen = field.GetArrayLength();
                                            var arr = new uint[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetUInt32();
                                            structItem[fieldIndex] = arr;
                                            break;
                                        }
                                    case NexColumnType.FloatArray:
                                        {
                                            ThrowIfStructElemNotValueKind(field, JsonValueKind.Array, col[fieldIndex], arrayIndex, fieldIndex);

                                            int arrLen = field.GetArrayLength();
                                            var arr = new float[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetSingle();
                                            structItem[fieldIndex] = arr;
                                            break;
                                        }
                                    default:
                                        throw new NotImplementedException($"Nested custom struct field type unsupported yet: {col[fieldIndex].Type}");
                                }
       
                                fieldIndex++;
                            }

                            array[arrayIndex++] = structItem;
                        }

                        return array;
                    }
                    else
                    {
                        return Array.Empty<object>();
                    }
                }
            default:
                throw new NotImplementedException("Could not parse cell - type {column.Type} not supported");
        }
    }

    private void ThrowIfStructElemNotValueKind(JsonElement jsonElement, JsonValueKind expectedKind, NexStructColumn nexColumn, int arrayIndex, int fieldIndex)
    {
        if (jsonElement.ValueKind != expectedKind)
            throw new Exception($"Expected '{nexColumn.Type}' type in struct array index {arrayIndex}, struct item {fieldIndex}, got '{jsonElement.ValueKind}' from json.");
    }

    public void Dispose()
    {
        _con.Dispose();
    }
}