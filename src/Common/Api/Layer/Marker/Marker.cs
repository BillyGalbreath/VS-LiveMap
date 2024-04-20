using System;
using JetBrains.Annotations;
using livemap.Common.Api.Json;
using LiveMap.Common.Api.Layer.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LiveMap.Common.Api.Layer.Marker;

/// <summary>
/// Represents a marker on the map
/// </summary>
/// <typeparam name="T">The type of marker this object represents</typeparam>
[PublicAPI]
public abstract class Marker<T> : JsonSerializable<T> where T : Marker<T> {
    /// <summary>
    /// Type identifier for the marker
    /// </summary>
    [JsonProperty(Order = 0)]
    public string Type { get; }

    /// <summary>
    /// Unique id for the marker
    /// </summary>
    [JsonProperty(Order = 1)]
    public string Id { get; }

    /// <summary>
    /// Optional settings
    /// </summary>
    [JsonProperty(Order = 2)]
    public Options.Options? Options { get; set; }

    /// <summary>
    /// Optional tooltip settings
    /// </summary>
    [JsonProperty(Order = 3)]
    public TooltipOptions? Tooltip { get; set; }

    /// <summary>
    /// Optional popup settings
    /// </summary>
    [JsonProperty(Order = 4)]
    public PopupOptions? Popup { get; set; }

    protected Marker(string type, string id) {
        Type = type;
        Id = id;
    }

    /// <inheritdoc/>
    public string ToJson() {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }

    /// <inheritdoc/>
    public static T FromJson(string json) {
        try {
            return JsonConvert.DeserializeObject<T>(json) ?? throw new NullReferenceException("null");
        } catch (Exception e) {
            Console.Error.WriteLine($"HMMM: {e}");
            throw new JsonSerializationException($"Error deserializing marker json ({json})", e);
        }
    }
}
