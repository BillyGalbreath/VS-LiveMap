using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Layers {
    public Players Players { get; set; } = new();

    public Spawn Spawn { get; set; } = new();
}
