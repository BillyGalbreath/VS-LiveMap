using JetBrains.Annotations;

namespace livemap.data;

[PublicAPI]
public class Config {
    public Httpd Httpd { get; set; } = new();

    public Web Web { get; set; } = new();

    public Zoom Zoom { get; set; } = new();
}
