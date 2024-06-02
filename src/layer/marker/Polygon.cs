using System;
using livemap.data;
using livemap.layer.marker.options.type;
using Newtonsoft.Json;

namespace livemap.layer.marker;

/// <summary>
/// The polygon marker is used to draw polygon overlays on the map
/// </summary>
/// <remarks>
/// This is mostly used to show more complex shaped areas/sections of the world such as protected claims, cities, nations, etc.<br/>
/// You can make more basic shapes with the <see cref="Rectangle"/>
/// </remarks>
public class Polygon : Marker {
    /// <summary>
    /// Absolute (not relative to spawn) world coordinates for the polygon's points
    /// </summary>
    [JsonProperty(Order = -1)]
    public Point[][][] Points { get; set; }

    /// <inheritdoc cref="PolygonOptions"/>
    [JsonProperty(Order = 10)]
    public new PolygonOptions? Options { get; set; }

    /// <summary>
    /// Create a new polygon at 0,0 with a random id
    /// </summary>
    public Polygon() : this(Guid.NewGuid().ToString(), new Point(0, 0)) { }

    /// <summary>
    /// Create a new polygon
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polygon's points</param>
    /// <param name="options">Optional settings for the polygon</param>
    public Polygon(string id, Point points, PolygonOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polygon
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polygon's points</param>
    /// <param name="options">Optional settings for the polygon</param>
    public Polygon(string id, Point[] points, PolygonOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polygon
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polygon's points</param>
    /// <param name="options">Optional settings for the polygon</param>
    public Polygon(string id, Point[][] points, PolygonOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polygon
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polygon's points</param>
    /// <param name="options">Optional settings for the polygon</param>
    public Polygon(string id, Point[][][] points, PolygonOptions? options = null) : base("polygon", id) {
        Points = points;
        Options = options;
    }

    /// <inheritdoc cref="Marker.FromJson{T}"/>
    public static Polygon FromJson(string json) {
        return FromJson<Polygon>(json);
    }
}
