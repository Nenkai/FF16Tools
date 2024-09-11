using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex;

public class JsonByteArrayConverter : JsonConverter<byte[]>
{
    public static readonly JsonByteArrayConverter Instance = new JsonByteArrayConverter();

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value.AsEnumerable());

    public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.String => reader.GetBytesFromBase64(),
            JsonTokenType.StartArray => JsonSerializer.Deserialize<List<byte>>(ref reader)!.ToArray(),
            JsonTokenType.Null => null,
            _ => throw new JsonException(),
        };
}
