using JetBrains.Annotations;
using Newtonsoft.Json;

namespace livemap.common.layer.marker.options.type;

/// <summary>
/// Optional settings for the <see cref="Ellipse"/> marker
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
