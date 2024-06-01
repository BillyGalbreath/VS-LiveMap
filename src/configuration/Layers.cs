using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Layers {
    public Players Players { get; set; } = new();

    public Spawn Spawn { get; set; } = new();

    public Traders Traders { get; set; } = new();

    public Translocators Translocators { get; set; } = new();
}
