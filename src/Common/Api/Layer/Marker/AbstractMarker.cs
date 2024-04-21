using System;
using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LiveMap.Common.Api.Layer.Marker;

/// <summary>
/// Represents a marker on the map
/// </summary>
[PublicAPI]
public abstract class AbstractMarker {
    /// <summary>
    /// Type identifier for the marker
    /// </summary>
    [JsonProperty(Order = -10)]
    public string Type { get; }

    /// <summary>
    /// Unique id for the marker
    /// </summary>
    [JsonProperty(Order = -9)]
    public string Id { get; }

    /// <summary>
    /// Optional settings
    /// </summary>
    [JsonProperty(Order = 10)]
    protected BaseOptions? Options { get; set; }

    /// <summary>
    /// Optional tooltip settings
    /// </summary>
    [JsonProperty(Order = 11)]
    public TooltipOptions? Tooltip { get; set; }

    /// <summary>
    /// Optional popup settings
    /// </summary>
    [JsonProperty(Order = 12)]
    public PopupOptions? Popup { get; set; }

    protected AbstractMarker(string type, string id) {
        Type = type;
        Id = id;
    }

    /// <summary>
    /// Serializes this object to a JSON string
    /// </summary>
    /// <returns>JSON string representing this object</returns>
    /// <exception cref="Newtonsoft.Json.JsonSerializationException">Error serializing object to json</exception>
    public string ToJson() {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }

    /// <summary>
    /// Deserializes a JSON string into an instance of this object
    /// </summary>
    /// <param name="json">JSON string representing this object</param>
    /// <returns>A new instance of this object</returns>
    /// <exception cref="Newtonsoft.Json.JsonSerializationException">Error deserializing json to object</exception>
    public static T FromJson<T>(string json) {
        try {
            return JsonConvert.DeserializeObject<T>(json) ?? throw new NullReferenceException("null");
        } catch (Exception) {
            Console.Error.WriteLine($"Error deserializing marker json ({json})");
            throw;
        }
    }
}
