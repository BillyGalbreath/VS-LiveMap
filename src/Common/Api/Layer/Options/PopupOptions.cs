using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options;

/// <summary>
/// Optional settings for popup overlays
/// </summary>
[PublicAPI]
public class PopupOptions : DivOverlayOptions {
    /// <summary>
    /// Maximum width of the popup, in pixels
    /// </summary>
    [JsonProperty(Order = 0)]
    public double? MaxWidth { get; set; }

    /// <summary>
    /// Minimum width of the popup, in pixels
    /// </summary>
    [JsonProperty(Order = 1)]
    public double? MinWidth { get; set; }

    /// <summary>
    /// If set, creates a scrollable container of the given height inside a popup if its content exceeds it
    /// </summary>
    /// <remarks>
    /// The scrollable container can be styled using the <c>leaflet-popup-scrolled</c> CSS class selector
    /// </remarks>
    [JsonProperty(Order = 2)]
    public double? MaxHeight { get; set; }

    /// <summary>
    /// Set it to <see langword="false"/> if you don't want the map to do panning animation to fit the opened popup
    /// </summary>
    [JsonProperty(Order = 3)]
    public bool? AutoPan { get; set; }

    /// <summary>
    /// The margin between the popup and the top left corner of the map view after autopanning was performed
    /// </summary>
    [JsonProperty(Order = 4)]
    public Point? AutoPanPaddingTopLeft { get; set; }

    /// <summary>
    /// The margin between the popup and the bottom right corner of the map view after autopanning was performed
    /// </summary>
    [JsonProperty(Order = 5)]
    public Point? AutoPanPaddingBottomRight { get; set; }

    /// <summary>
    /// Equivalent of setting both top left and bottom right autopan padding to the same value
    /// </summary>
    [JsonProperty(Order = 6)]
    public Point? AutoPanPadding { get; set; }

    /// <summary>
    /// Set it to <see langword="true"/> if you want to prevent users from panning the popup off of the screen while it is open
    /// </summary>
    [JsonProperty(Order = 7)]
    public bool? KeepInView { get; set; }

    /// <summary>
    /// Controls the presence of a close button in the popup
    /// </summary>
    [JsonProperty(Order = 8)]
    public bool? CloseButton { get; set; }

    /// <summary>
    /// Set it to <see langword="false"/> if you want to override the default behavior of the popup closing when another popup is opened
    /// </summary>
    [JsonProperty(Order = 9)]
    public bool? AutoClose { get; set; }

    /// <summary>
    /// Set it to <see langword="false"/> if you want to override the default behavior of the ESC key for closing of the popup
    /// </summary>
    [JsonProperty(Order = 10)]
    public bool? CloseOnEscapeKey { get; set; }

    /// <summary>
    /// Set it if you want to override the default behavior of the popup closing when user clicks on the map
    /// </summary>
    [JsonProperty(Order = 11)]
    public bool? CloseOnClick { get; set; }
}
