using System.Text.Json;
using Shared.Dtos;

namespace Shared;

/// <summary>
/// Interface for JSON parsing operations.
/// </summary>
public interface IJsonParser
{
    /// <summary>
    /// Serializes an object to JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The JSON string representation.</returns>
    string SerializeJson<T>(T obj);
    
    /// <summary>
    /// Serializes an object to JsonElement.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The JsonElement.</returns>
    JsonElement SerializeToJsonElement<T>(T obj);

    /// <summary>
    /// Serializes an object to byte array.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The JSON string representation.</returns>
    byte[] SerializeToBytes<T>(T obj);
    
    /// <summary>
    /// Deserializes a JSON string to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    T? DeserializeJson<T>(string json);
    
    /// <summary>
    /// Deserializes a JsonElement to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="element">The JsonElement to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    T? DeserializeJsonElement<T>(JsonElement element);
}

/// <summary>
/// Provides methods for serializing and deserializing JSON.
/// </summary>
public class JsonParser : IJsonParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new TwoDimensionalArrayConverter<TileDto>() }
    };
    
    /// <inheritdoc/>
    public string SerializeJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj,Options);
    }

    /// <inheritdoc/>
    public JsonElement SerializeToJsonElement<T>(T obj)
    {
        return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(obj,Options));
    }
    
    /// <inheritdoc/>
    public byte[] SerializeToBytes<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj,Options);
    }
    
   /// <inheritdoc/>
    public T? DeserializeJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json,Options);
    }

    /// <inheritdoc/>
    public T? DeserializeJsonElement<T>(JsonElement element)
    {
        return element.Deserialize<T>(Options);
    }
}