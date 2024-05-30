using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Spawn {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;
}
