using JetBrains.Annotations;
using livemap.Common.Api.Json;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options;

/// <summary>
/// A set of options shared between vector overlays<br/>
/// (<see cref="Layer.Marker.Polygon"/>, <see cref="Layer.Marker.Polyline"/>, <see cref="Layer.Marker.Circle"/>, etc.)
/// </summary>
/// <remarks>
/// Do not use it directly
/// </remarks>
[PublicAPI]
public abstract class PathOptions : LayerOptions {
    /// <summary>
    /// Whether to draw stroke along the path. Set it to <see langword="false"/> to disable borders on polygons or circles
    /// </summary>
    /// <remarks>
    /// Defaults to <see langword="true"/> if not set
    /// </remarks>
    [JsonProperty(Order = 0)]
    public bool? Stroke { get; set; }

    /// <summary>
    /// Stroke color
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"#3388FF"</c> if not set
    /// </remarks>
    [JsonConverter(typeof(ColorJsonConverter))]
    [JsonProperty(Order = 1)]
    public uint? Color { get; set; }

    /// <summary>
    /// Stroke width, in pixels
    /// </summary>
    /// <remarks>
    /// Defaults to <c>3</c> if not set
    /// </remarks>
    [JsonProperty(Order = 2)]
    public int? Weight { get; set; }

    /// <summary>
    /// Stroke opacity
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1.0</c> if not set
    /// </remarks>
    [JsonProperty(Order = 3)]
    public double? Opacity { get; set; }

    /// <summary>
    /// A string that defines shape to be used at the end of the stroke
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"round"</c> if not set
    /// </remarks>
    /// <seealso href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linecap">MDN Web Docs</seealso>
    [JsonProperty(Order = 4)]
    public string? LineCap { get; set; }

    /// <summary>
    /// A string that defines shape to be used at the corners of the stroke
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"round"</c> if not set
    /// </remarks>
    /// <seealso href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-linejoin">MDN Web Docs</seealso>
    [JsonProperty(Order = 5)]
    public string? LineJoin { get; set; }

    /// <summary>
    /// A string that defines the stroke dash pattern
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c> if not set
    /// </remarks>
    /// <seealso href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-dasharray">MDN Web Docs</seealso>
    [JsonProperty(Order = 6)]
    public string? DashArray { get; set; }

    /// <summary>
    /// A string that defines the distance into the dash pattern to start the dash
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c> if not set
    /// </remarks>
    /// <seealso href="https://developer.mozilla.org/docs/Web/SVG/Attribute/stroke-dashoffset">MDN Web Docs</seealso>
    [JsonProperty(Order = 7)]
    public string? DashOffset { get; set; }

    /// <summary>
    /// Whether to fill the path with color. Set it to <see langword="false"/> to disable filling on polygons or circles
    /// </summary>
    /// <remarks>
    /// Defaults to <see langword="true"/> if not set, except for <see cref="Layer.Marker.Polyline"/>
    /// </remarks>
    [JsonProperty(Order = 8)]
    public bool? Fill { get; set; }

    /// <summary>
    /// Fill color
    /// </summary>
    /// <remarks>
    /// Defaults to the value of the <see cref="PathOptions.Color">Color</see> option if not set
    /// </remarks>
    [JsonConverter(typeof(ColorJsonConverter))]
    [JsonProperty(Order = 9)]
    public uint? FillColor { get; set; }

    /// <summary>
    /// Fill opacity
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.2</c> if not set
    /// </remarks>
    [JsonProperty(Order = 10)]
    public double? FillOpacity { get; set; }

    /// <summary>
    /// A string that defines how the inside of a shape is determined
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"evenodd"</c> if not set
    /// </remarks>
    /// <seealso href="https://developer.mozilla.org/docs/Web/SVG/Attribute/fill-rule">MDN Web Docs</seealso>
    [JsonProperty(Order = 11)]
    public string? FillRule { get; set; }
}