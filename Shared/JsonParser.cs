using System.Text.Json;

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
}

/// <summary>
/// Provides methods for serializing and deserializing JSON.
/// </summary>
public class JsonParser : IJsonParser
{
    /// <inheritdoc/>
    public string SerializeJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }
    
    /// <inheritdoc/>
    public byte[] SerializeToBytes<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }
    
   /// <inheritdoc/>
    public T? DeserializeJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}