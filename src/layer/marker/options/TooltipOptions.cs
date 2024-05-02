using JetBrains.Annotations;
using livemap.data;
using Newtonsoft.Json;

namespace livemap.layer.marker.options;

/// <summary>
/// Optional settings for tooltip overlays
/// </summary>
[PublicAPI]
public class TooltipOptions : DivOverlayOptions {
    /// <summary>
    /// Direction where to open the tooltip
    /// </summary>
    /// <remarks>
    /// Possible values are:<br/>
    /// <c>right</c>, <c>left</c>, <c>top</c>, <c>bottom</c>, <c>center</c>, and <c>auto</c><br/>
    /// <br/>
    /// <c>auto</c> will dynamically switch between right and left according to the tooltip position on the map
    /// </remarks>
    [JsonProperty(Order = 0)]
    public string? Direction { get; set; }

    /// <summary>
    /// Whether to open the tooltip permanently or only on mouseover
    /// </summary>
    [JsonProperty(Order = 1)]
    public bool? Permanent { get; set; }

    /// <summary>
    /// If <see langword="true"/>, the tooltip will follow the mouse instead of being fixed at the feature center
    /// </summary>
    [JsonProperty(Order = 2)]
    public bool? Sticky { get; set; }

    /// <summary>
    /// Tooltip container opacity
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.9</c> (<c>0xE5</c>) if not set
    /// </remarks>
    [JsonProperty(Order = 3)]
    public Opacity? Opacity { get; set; }
}
