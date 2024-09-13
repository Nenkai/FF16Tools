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
using System.Text.Json;

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
    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonByteArrayConverter() } };

    public SQLiteToNexImporter(string sqliteFile, List<string> tablesToConvert = null, ILoggerFactory loggerFactory = null)
    {
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

        _logger.LogInformation("Exported to {directory}.", directory);
    }

    private void CreateTables()
    {
        var command = _con.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';";
        _logger.LogTrace(command.CommandText);

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

        _logger.LogInformation("Loaded {count} tables from SQLite", _tableLayouts.Count);
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
            for (int i = 0; i < tableLayout.Columns.Count; i++)
            {
                var column = tableLayout.Columns[i];

                if (!rows.Any(e => (string)e["ColumnName"] == column.Name))
                {
                    throw new InvalidDataException($"Column '{column.Name}' for table '{table.Key}' does not exist in sqlite table");
                }
            }

            while (reader.Read())
            {
                List<object> cells = [];
                uint rowId = 0, subId = 0, arrayIndex = 0;
                if (tableLayout.Type == NexTableType.Rows)
                {
                    rowId = (uint)(long)reader["RowID"];
                }
                else if (tableLayout.Type == NexTableType.RowSets)
                {
                    rowId = (uint)(long)reader["RowID"];
                    arrayIndex = (uint)(long)reader["ArrayIndex"];
                }
                else if (tableLayout.Type == NexTableType.DoubleKeyed)
                {
                    rowId = (uint)(long)reader["RowID"];
                    subId = (uint)(long)reader["SubID"];
                    arrayIndex = (uint)(long)reader["ArrayIndex"];
                }
                else
                    throw new NotImplementedException($"Table layout type {tableLayout.Type} not yet supported");

                for (int i = 0; i < tableLayout.Columns.Count; i++)
                {
                    var column = tableLayout.Columns[i];
                    object val = reader[column.Name];
                    object cell = ParseCell(tableLayout, column, val);
                    cells.Add(cell);
                }

                tableBuilder.AddRow(rowId, subId, arrayIndex, cells);
            }
        }
    }

    private static object ParseCell(NexTableLayout tableLayout, NexStructColumn column, object val)
    {
        switch (column.Type)
        {
            case NexColumnType.SByte:
                return (sbyte)(long)val;
            case NexColumnType.Byte:
                return (byte)(long)val;
            case NexColumnType.Short:
                return (short)(long)val;
            case NexColumnType.UShort:
                return (ushort)(long)val;
            case NexColumnType.Int:
                return (int)(long)val;
            case NexColumnType.UInt:
                return (uint)(long)val;
            case NexColumnType.Float:
                return (float)(double)val;
            case NexColumnType.Int64:
                return (long)val;
            case NexColumnType.Double:
                return (double)val;
            case NexColumnType.String:
                return (string)val;
            case NexColumnType.ByteArray:
                {
                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        byte[] arr = JsonSerializer.Deserialize<byte[]>(arrStr, _jsonSerializerOptions);
                        return arr;
                    }
                    else
                        return Array.Empty<int>();
                }
            case NexColumnType.IntArray:
                {
                    string arrStr = (string)val;
                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        int[] arr = JsonSerializer.Deserialize<int[]>(arrStr);
                        return arr;
                    }
                    else
                        return Array.Empty<int>();
                }
            case NexColumnType.FloatArray:
                {
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
                    string arrStr = (string)val;
                    var col = tableLayout.CustomStructDefinitions[column.StructTypeName];
                    List<object> array = [];

                    if (!string.IsNullOrEmpty(arrStr))
                    {
                        JsonElement obj = (JsonElement)JsonSerializer.Deserialize<object>(arrStr);
                        foreach (JsonElement item in obj.EnumerateArray())
                        {
                            var structItem = new List<object>();

                            int fieldIndex = 0;
                            foreach (JsonElement field in item.EnumerateArray())
                            {
                                switch (col[fieldIndex].Type)
                                {
                                    case NexColumnType.Short:
                                        structItem.Add(field.GetInt16());
                                        break;
                                    case NexColumnType.String:
                                        structItem.Add(field.GetString());
                                        break;
                                    case NexColumnType.Float:
                                        structItem.Add(field.GetSingle());
                                        break;
                                    case NexColumnType.Int:
                                        structItem.Add(field.GetInt32());
                                        break;
                                    case NexColumnType.ByteArray:
                                        {
                                            int arrLen = field.GetArrayLength();
                                            var arr = new byte[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetByte();
                                            structItem.Add(arr);
                                            break;
                                        }
                                    case NexColumnType.IntArray:
                                        {
                                            int arrLen = field.GetArrayLength();
                                            var arr = new int[arrLen];

                                            int j = 0;
                                            foreach (JsonElement elem in field.EnumerateArray())
                                                arr[j++] = elem.GetInt32();
                                            structItem.Add(arr);
                                            break;
                                        }
                                    default:
                                        throw new NotImplementedException($"Nested custom struct field type unsupported yet: {col[fieldIndex].Type}");
                                }
       
                                fieldIndex++;
                            }

                            array.Add(structItem);
                        }

                        return array;
                    }
                    else
                    {
                        return array;
                    }
                }
            default:
                throw new NotImplementedException($"Could not parse cell - type {column.Type} not supported");
        }
    }

    public void Dispose()
    {
        _con.Dispose();
    }
}