using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options.Marker;

/// <summary>
/// Optional settings for the <see cref="Layer.Marker.Ellipse"/> marker
/// </summary>
[PublicAPI]
public class EllipseOptions : PathOptions {
    /// <summary>
    /// The semi-major and semi-minor axis in blocks
    /// </summary>
    [JsonProperty(Order = 0)]
    public double[]? Radii { get; set; }

    /// <summary>
    /// Rotation angle, in degrees, clockwise
    /// </summary>
    [JsonProperty(Order = 1)]
    public double? Tilt { get; set; }
}
