using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options;

/// <summary>
/// Optional settings for <see cref="TooltipOptions"/> and <see cref="PopupOptions"/> overlays
/// </summary>
[PublicAPI]
public abstract class OverlayOptions : LayerOptions {
    /// <summary>
    /// The offset of this overlay position
    /// </summary>
    [JsonProperty(Order = 0)]
    public Point? Offset { get; set; }

    /// <summary>
    /// A custom CSS class name to assign to this overlay
    /// </summary>
    [JsonProperty(Order = 1)]
    public string? ClassName { get; set; }

    /// <summary>
    /// Sets the HTML contents of this overlay
    /// </summary>
    [JsonProperty(Order = 2)]
    public string? Content { get; set; }
}
