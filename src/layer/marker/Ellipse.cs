using System;
using livemap.data;
using livemap.layer.marker.options.type;
using Newtonsoft.Json;

namespace livemap.layer.marker;

/// <summary>
/// The ellipse marker is used to draw ellipse overlays on the map
/// </summary>
/// <remarks>
/// You can make perfect circles with the <see cref="Circle"/>
/// </remarks>
public class Ellipse : Marker {
    /// <summary>
    /// Absolute (not relative to spawn) world coordinates for the ellipse's center
    /// </summary>
    [JsonProperty(Order = -1)]
    public Point Point { get; set; }

    /// <inheritdoc cref="EllipseOptions"/>
    [JsonProperty(Order = 10)]
    public new EllipseOptions? Options { get; set; }

    /// <summary>
    /// Create a new ellipse at 0,0 with a random id
    /// </summary>
    public Ellipse() : this(Guid.NewGuid().ToString(), new Point(0, 0)) { }

    /// <summary>
    /// Create a new ellipse
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="point">Absolute (not relative to spawn) world coordinates for the ellipse's center</param>
    /// <param name="options">Optional settings for the ellipse</param>
    public Ellipse(string id, Point point, EllipseOptions? options = null) : base("ellipse", id) {
        Point = point;
        Options = options;
    }

    /// <inheritdoc cref="Marker.FromJson{T}"/>
    public static Ellipse FromJson(string json) {
        return FromJson<Ellipse>(json);
    }
}
