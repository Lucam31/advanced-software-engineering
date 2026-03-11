using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public class TwoDimensionalArrayConverter<T> : JsonConverter<T[,]>
{
    public override T[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jagged = JsonSerializer.Deserialize<T[][]>(ref reader, options)!;
        var rows = jagged.Length;
        var cols = jagged[0].Length;
        var result = new T[rows, cols];
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
            result[i, j] = jagged[i][j];
        return result;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        var rows = value.GetLength(0);
        var cols = value.GetLength(1);
        writer.WriteStartArray();
        for (var i = 0; i < rows; i++)
        {
            writer.WriteStartArray();
            for (var j = 0; j < cols; j++)
                JsonSerializer.Serialize(writer, value[i, j], options);
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}
