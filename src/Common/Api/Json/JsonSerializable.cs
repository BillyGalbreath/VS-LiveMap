using JetBrains.Annotations;

namespace livemap.Common.Api.Json;

/// <summary>
/// Represents an object that can be serialized to/from json
/// </summary>
/// <typeparam name="T">The type of object this represents</typeparam>
[PublicAPI]
public interface JsonSerializable<out T> where T : JsonSerializable<T> {
    /// <summary>
    /// Serializes this object to a JSON string
    /// </summary>
    /// <returns>JSON string representing this object</returns>
    /// <exception cref="Newtonsoft.Json.JsonSerializationException">Error serializing object to json</exception>
    public string ToJson();

    /// <summary>
    /// Deserializes a JSON string into an instance of this object
    /// </summary>
    /// <param name="json">JSON string representing this object</param>
    /// <returns>A new instance of this object</returns>
    /// <exception cref="Newtonsoft.Json.JsonSerializationException">Error deserializing json to object</exception>
    public abstract static T FromJson(string json);
}
