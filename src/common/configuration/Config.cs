namespace livemap.common.configuration;

public class Config {
    public Httpd Httpd { get; set; } = new();

    public Web Web { get; set; } = new();

    public Zoom Zoom { get; set; } = new();
}
