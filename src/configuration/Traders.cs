using JetBrains.Annotations;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

[PublicAPI]
public class Traders {
    public bool Enabled { get; set; } = false;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "",
        Alt = "",
        IconUrl = "#svg-trader",
        IconSize = new Point(16, 16)
    };
}
