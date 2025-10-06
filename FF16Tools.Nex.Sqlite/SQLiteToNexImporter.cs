using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

using FF16Tools.Files.Nex;
using FF16Tools.Files.Nex.Entities;
using System.Diagnostics.CodeAnalysis;

namespace FF16Tools.Nex.Sqlite;

/// <summary>
/// Game database to sqlite exporter (disposable object).
/// </summary>
public class SQLiteToNexImporter : IDisposable
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger? _logger;

    private readonly Dictionary<string, NexDataFileBuilder> _tableBuilders = [];
    private readonly Dictionary<string, NexTableLayout> _tableLayouts = [];

    private readonly List<string>? _tablesToConvert;
    private readonly string _sqliteFile = "<no table>";

    private readonly string _codeName;

    private SqliteConnection? _con;

    private Dictionary<string, NexUnionType> _unionMap = [];

    // We don't want byte arrays to be converted to base64.
    private static JsonSerializerOptions _jsonSerializerOptions = new() { Converters = { new JsonByteArrayConverter() } };

    private string? _lastTable;

    /// <summary>
    /// SQLite to Nex importer.
    /// </summary>
    /// <param name="sqliteFile">Path to sqlite file.</param>
    /// <param name="version">Version which should match the game. Should be at least 1.0.0.</param>
    /// <param name="tablesToConvert">Tables to convert. If null is provided, all tables will be converted.</param>
    /// <param name="loggerFactory">Logger factory, for logging.</param>
    public SQLiteToNexImporter(string sqliteFile, Version version, string codeName, List<string>? tablesToConvert = null, ILoggerFactory? loggerFactory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqliteFile, nameof(sqliteFile));
        ArgumentNullException.ThrowIfNull(version, nameof(version));
        ArgumentNullException.ThrowIfNull(codeName, nameof(codeName));

        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        _tablesToConvert = tablesToConvert;
        _sqliteFile = sqliteFile;

        _codeName = codeName;

        _loggerFactory = loggerFactory;
        if (loggerFactory is not null)
            _logger = loggerFactory.CreateLogger(GetType().ToString());
    }

    /// <summary>
    /// Reads the sqlite file.
    /// </summary>
    public void ReadSqlite()
    {
        _con = new SqliteConnection($"Data Source={_sqliteFile}");
        _con.Open();

        ReadUnionTable();
        CreateTables();
        FillTables();

        _con.Close();
    }

    /// <summary>
    /// Saves all read tables from sqlite as nex files to the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    public void SaveTo(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory, nameof(directory));

        Directory.CreateDirectory(directory);

        foreach (var table in _tableBuilders)
        {
            string nxdPath = Path.Combine(directory, $"{table.Key.ToLower().Replace('-', '.')}.nxd");

            _logger?.LogInformation("Writing {file}", nxdPath);
            using var fs = new FileStream(nxdPath, FileMode.Create);
            table.Value.Write(fs);
        }

        _logger?.LogInformation("Exported to {directory}.", directory);
    }

    private void ReadUnionTable()
    {
        var command = _con!.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='_uniontypes';";
        string? tableName = command.ExecuteScalar() as string;
        if (string.IsNullOrEmpty(tableName))
        {
            _logger?.LogWarning("Table '_uniontypes' is missing.");
            return;
        }

        command = _con.CreateCommand();
        command.CommandText = $"SELECT * FROM _uniontypes;";
        _logger?.LogTrace("{command}", command.CommandText);

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            long id = (long)reader["Id"];
            string name = (string)reader["Name"];

            if (id < 0)
                continue;

            _unionMap.TryAdd(name, (NexUnionType)id);
        }
    }

    private void CreateTables()
    {
        var command = _con!.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';";
        _logger?.LogTrace("{command}", command.CommandText);

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            string tableName = (string)reader["name"];
            string[] spl = tableName.Split("-");

            string tableNameWithoutLocale = tableName;
            if (spl.Length >= 2)
                tableNameWithoutLocale = spl[0];

            if (!TableMappingReader.LayoutExists(tableNameWithoutLocale, _codeName))
            {
                _logger?.LogInformation("Skipping table {tableName}, no layout exists", tableName);
                continue;
            }

            if (_tablesToConvert is not null && _tablesToConvert.Count > 0)
            {
                if (!_tablesToConvert.Contains(tableName))
                    continue;
            }

            _logger?.LogInformation("Fetching table {tableName}", tableName);

            var layout = TableMappingReader.ReadTableLayout(tableNameWithoutLocale, new Version(1, 0, 0), _codeName);
            _tableBuilders.Add(tableName, new NexDataFileBuilder(layout, _loggerFactory));
            _tableLayouts.Add(tableName, layout);
        }

        _logger?.LogInformation("Loaded {count} tables from SQLite", _tableLayouts.Count);
    }

    private void FillTables()
    {
        foreach (KeyValuePair<string, NexDataFileBuilder> table in _tableBuilders)
        {
            _lastTable = table.Key;

            var command = _con!.CreateCommand();
            command.CommandText = $"SELECT * FROM \"{table.Key}\";";
            _logger?.LogTrace("{command}", command.CommandText);

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

            NexStructColumn? lastColumn = null;
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
                        VerifyCellOrThrow(tableLayout, lastColumn, cell);

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

    private static void VerifyCellOrThrow(NexTableLayout tableLayout, NexStructColumn lastColumn, object cell)
    {
        // Check for NaNs
        if (lastColumn.Type == NexColumnType.Float)
        {
            if (cell is float f && float.IsNaN(f))
                throw new InvalidDataException("Float is invalid (NaN)");
        }
        else if (lastColumn.Type == NexColumnType.FloatArray)
        {
            float[] arr = (float[])cell;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] is float f && float.IsNaN(f))
                    throw new InvalidDataException($"Float is invalid (NaN) at array index {i}");
            }
        }
        else if (lastColumn.Type == NexColumnType.CustomStructArray)
        {
            object[] array = (object[])cell;
            for (int i = 0; i < array.Length; i++)
            {
                object[] elem = (object[])array[i];
                for (int j = 0; j < elem.Length; j++)
                {
                    NexTableColumnStruct tableStruct = tableLayout.CustomStructDefinitions[lastColumn.StructTypeName!];
                    VerifyCellOrThrow(tableLayout, tableStruct.Columns[j], elem[j]);
                }
            }
        }
    }

    private static object ParseCellArray<T>(object val)
    {
        if (val is DBNull)
            return Array.Empty<T>();

        string arrStr = (string)val;
        if (!string.IsNullOrEmpty(arrStr))
        {
            T[]? arr = JsonSerializer.Deserialize<T[]>(arrStr, _jsonSerializerOptions);
            if (arr is null)
                return Array.Empty<T>();

            return arr;
        }
        else
            return Array.Empty<T>();
    }

    private object ParseCellCustomStructArray<T>(JsonElement field, NexStructColumn column, int arrayIndex, int fieldIndex, Func<JsonElement, T> getter)
    {
        ThrowIfStructElemNotValueKind(field, JsonValueKind.Array, column, arrayIndex, fieldIndex);

        int arrLen = field.GetArrayLength();
        var arr = new T[arrLen];

        int j = 0;
        foreach (JsonElement elem in field.EnumerateArray())
            arr[j++] = getter(elem);
        return arr;
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
                        ThrowTableError($"Unable to parse hexadecimal uint '{hexStr}' at {column.Name}");

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
            case NexColumnType.NexUnionKey32:
            case NexColumnType.NexUnionKey16:
                {
                    string str = (string)val;
                    return ParseUnion(column, str);
                }
            case NexColumnType.NexUnionKey32Array:
                {
                    string str = (string)val;
                    if (string.IsNullOrWhiteSpace(str))
                        return Array.Empty<NexUnionKey>();

                    if (str[0] != '[' || str[^1] != ']')
                        ThrowTableError($"Union array is malformed at column {column.Name} - expected an array, got '{str}'");

                    ReadOnlySpan<char> contents = str.AsSpan(1, str.Length - 2);

                    int count = 0;
                    if (contents.Length > 0)
                    {
                        foreach (var _ in contents.Split(','))
                            count++;

                        var arr = new NexUnionKey[count];
                        int i = 0;
                        foreach (Range elemRange in contents.Split(','))
                        {
                            ReadOnlySpan<char> elemSpan = contents[elemRange];
                            arr[i++] = ParseUnion(column, elemSpan);
                        }

                        return arr;
                    }
                    else
                        return Array.Empty<NexUnionKey>();

                }
            case NexColumnType.ByteArray:
                {
                    return ParseCellArray<byte>(val);
                }
            case NexColumnType.ShortArray:
                {
                    return ParseCellArray<short>(val);
                }
            case NexColumnType.IntArray:
                {
                    return ParseCellArray<int>(val);
                }
            case NexColumnType.UIntArray:
                {
                    return ParseCellArray<uint>(val);
                }
            case NexColumnType.FloatArray:
                {
                    return ParseCellArray<float>(val);
                }
            case NexColumnType.StringArray:
                {
                    return ParseCellArray<string>(val);
                }
            case NexColumnType.CustomStructArray:
                {
                    if (val is DBNull)
                        return Array.Empty<object>();

                    string arrStr = (string)val;
                    NexTableColumnStruct customStruct = tableLayout.CustomStructDefinitions[column.StructTypeName!];

                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        JsonElement obj = (JsonElement)JsonSerializer.Deserialize<object>(arrStr);
                        object[] array = new object[obj.GetArrayLength()];

                        int arrayIndex = 0;
                        foreach (JsonElement item in obj.EnumerateArray())
                        {
                            var structItem = new object[customStruct.Columns.Count];
                            int itemsInJson = item.GetArrayLength();
                            if (itemsInJson != customStruct.Columns.Count)
                                ThrowTableError($"Error in column {column.Name} - expected {customStruct.Columns.Count} fields in struct array index {arrayIndex}, got {itemsInJson} in json.");

                            int fieldIndex = 0;
                            foreach (JsonElement field in item.EnumerateArray())
                            {
                                switch (customStruct.Columns[fieldIndex].Type)
                                {
                                    case NexColumnType.Byte:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetByte();
                                        break;
                                    case NexColumnType.SByte:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetSByte();
                                        break;
                                    case NexColumnType.Short:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetInt16();
                                        break;
                                    case NexColumnType.UShort:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetUInt16();
                                        break;
                                    case NexColumnType.String:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.String, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetString()!;
                                        break;
                                    case NexColumnType.Float:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetSingle();
                                        break;
                                    case NexColumnType.Int:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetInt32();
                                        break;
                                    case NexColumnType.UInt:
                                    case NexColumnType.HexUInt:
                                        ThrowIfStructElemNotValueKind(field, JsonValueKind.Number, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex);
                                        structItem[fieldIndex] = field.GetUInt32();
                                        break;
                                    case NexColumnType.NexUnionKey32:
                                    case NexColumnType.NexUnionKey16:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetInt32());
                                            break;
                                        }
                                    case NexColumnType.ByteArray:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetByte());
                                            break;
                                        }
                                    case NexColumnType.ShortArray:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetInt32());
                                            break;
                                        }
                                    case NexColumnType.IntArray:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetInt32());
                                            break;
                                        }
                                    case NexColumnType.UIntArray:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetUInt32());
                                            break;
                                        }
                                    case NexColumnType.FloatArray:
                                        {
                                            structItem[fieldIndex] = ParseCellCustomStructArray(field, customStruct.Columns[fieldIndex], arrayIndex, fieldIndex, e => e.GetSingle());
                                            break;
                                        }
                                    default:
                                        ThrowTableError($"Nested custom struct field type unsupported yet: {customStruct.Columns[fieldIndex].Type}");
                                        break;
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
                ThrowTableError($"Could not parse cell - type {column.Type} not supported");
                throw new UnreachableException();
        }
    }

    private NexUnionKey ParseUnion(NexStructColumn column, ReadOnlySpan<char> val)
    {
        int idx = 0;

        NexUnionType? type = null;
        int id = 0;
        foreach (Range elemRange in val.Split(':'))
        {
            if (idx > 2)
                ThrowTableError($"Union is malformed at column {column.Name} - too many elements (source of error: {val})");

            ReadOnlySpan<char> elemSpan = val[elemRange];
            if (idx == 0)
            {
                // Check if the database already has mappings
                if (_unionMap.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(elemSpan, out NexUnionType value))
                    type = value;

                // If not, try to parse our local enum.
                if (type is null)
                {
                    if (Enum.TryParse(elemSpan, out NexUnionType value_))
                        type = value_;
                }

                // If not, try to parse it as a number
                if (type is null)
                {
                    if (!ushort.TryParse(elemSpan, out ushort value_))
                        ThrowTableError($"Union is malformed at column {column.Name} - unable to parse '{elemSpan}' as union type or number");

                    type = (NexUnionType)value_;
                }
            }
            else if (idx == 1)
            {
                if (!int.TryParse(elemSpan, out int value))
                    ThrowTableError($"Union is malformed at column {column.Name} - unable to parse '{elemSpan}' as union id");

                id = value;
            }

            idx++;
        }

        if (idx != 2)
            ThrowTableError($"Union is malformed at column {column.Name} - not enough elements, expected type:id");

        return new NexUnionKey(type!.Value, id);
    }

    private void ThrowIfStructElemNotValueKind(JsonElement jsonElement, JsonValueKind expectedKind, NexStructColumn nexColumn, int arrayIndex, int fieldIndex)
    {
        if (jsonElement.ValueKind != expectedKind)
            throw new Exception($"{_lastTable}: Expected '{nexColumn.Type}' type in struct array index {arrayIndex}, struct item {fieldIndex}, got '{jsonElement.ValueKind}' from json.");
    }

    [DoesNotReturn]
    private void ThrowTableError(string message)
    {
        throw new Exception($"{_lastTable}: {message}");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _con?.Dispose();
    }
}