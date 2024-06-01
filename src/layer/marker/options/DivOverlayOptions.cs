using JetBrains.Annotations;
using livemap.data;
using Newtonsoft.Json;

namespace livemap.layer.marker.options;

/// <summary>
/// Optional settings for <see cref="TooltipOptions"/> and <see cref="PopupOptions"/> overlays
/// </summary>
[PublicAPI]
public class DivOverlayOptions : InteractiveLayerOptions {
    /// <summary>
    /// The offset of this overlay position
    /// </summary>
    [JsonProperty(Order = 100)]
    public Point? Offset { get; set; }

    /// <summary>
    /// A custom CSS class name to assign to this overlay
    /// </summary>
    [JsonProperty(Order = 101)]
    public string? ClassName { get; set; }

    /// <summary>
    /// Sets the HTML contents of this overlay
    /// </summary>
    [JsonProperty(Order = 102)]
    public string? Content { get; set; }
}
