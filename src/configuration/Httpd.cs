using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Httpd {
    public bool Enabled { get; set; } = true;

    public int Port { get; set; } = 8080;
}
