using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class Translocators {
    public bool Enabled { get; set; } = false;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "",
        Alt = "",
        IconUrl = "#svg-spiral",
        IconSize = new Point(16, 16)
    };
}
