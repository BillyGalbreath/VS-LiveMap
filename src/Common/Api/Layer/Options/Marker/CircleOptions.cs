using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Marker;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options.Marker;

/// <summary>
/// Optional settings for the <see cref="Circle"/> marker
/// </summary>
[PublicAPI]
public class CircleOptions : PathOptions {
    /// <summary>
    /// Radius of the circle, in blocks
    /// </summary>
    [JsonProperty(Order = 0)]
    public double? Radius { get; set; }
}
