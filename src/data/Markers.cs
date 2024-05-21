using JetBrains.Annotations;

namespace livemap.data;

[PublicAPI]
public class Markers {
    public bool Players { get; set; } = true;

    public bool Spawn { get; set; } = true;
}
