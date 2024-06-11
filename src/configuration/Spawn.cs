using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class Spawn {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "",
        Alt = "Spawn",
        IconUrl = "#svg-house",
        IconSize = new Point(16, 16)
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "Spawn"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; }
}
