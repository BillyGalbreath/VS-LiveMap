using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Config {
    public Httpd Httpd { get; set; } = new();

    public Web Web { get; set; } = new();

    public Zoom Zoom { get; set; } = new();

    public Ui Ui { get; set; } = new();

    public Markers Markers { get; set; } = new();
}
