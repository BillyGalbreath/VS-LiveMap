using JetBrains.Annotations;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

[PublicAPI]
public class Spawn {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "Spawn",
        Alt = "Spawn",
        IconUrl = "#svg-house",
        IconSize = new Point(16, 16)
    };
}
