using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Zoom {
    public int Default { get; set; }

    public int MaxIn { get; set; } = -3;

    public int MaxOut { get; set; } = 8;
}