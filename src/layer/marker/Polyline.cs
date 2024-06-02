using System;
using livemap.data;
using livemap.layer.marker.options.type;
using Newtonsoft.Json;

namespace livemap.layer.marker;

/// <summary>
/// The polyline marker is used to draw polyline overlays on the map
/// </summary>
/// <remarks>
/// You can fill in these polyline shapes with the <see cref="Polygon"/>
/// </remarks>
public class Polyline : Marker {
    /// <summary>
    /// Absolute (not relative to spawn) world coordinates for the polyline's points
    /// </summary>
    [JsonProperty(Order = -1)]
    public Point[][][] Points { get; set; }

    /// <inheritdoc cref="PolylineOptions"/>
    [JsonProperty(Order = 10)]
    public new PolylineOptions? Options { get; set; }

    /// <summary>
    /// Create a new polyline at 0,0 with a random id
    /// </summary>
    public Polyline() : this(Guid.NewGuid().ToString(), new Point(0, 0)) { }

    /// <summary>
    /// Create a new polyline
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polyline's points</param>
    /// <param name="options">Optional settings for the polyline</param>
    public Polyline(string id, Point points, PolylineOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polyline
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polyline's points</param>
    /// <param name="options">Optional settings for the polyline</param>
    public Polyline(string id, Point[] points, PolylineOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polyline
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polyline's points</param>
    /// <param name="options">Optional settings for the polyline</param>
    public Polyline(string id, Point[][] points, PolylineOptions? options = null) : this(id, new[] { points }, options) { }

    /// <summary>
    /// Create a new polyline
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="points">Absolute (not relative to spawn) world coordinates for the polyline's points</param>
    /// <param name="options">Optional settings for the polyline</param>
    public Polyline(string id, Point[][][] points, PolylineOptions? options = null) : base("polyline", id) {
        Points = points;
        Options = options;
    }

    /// <inheritdoc cref="Marker.FromJson{T}"/>
    public static Polyline FromJson(string json) {
        return FromJson<Polyline>(json);
    }
}
