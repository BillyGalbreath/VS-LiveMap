using JetBrains.Annotations;

namespace livemap.configuration;

[PublicAPI]
public class Players {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 1;

    public bool DefaultShowLayer { get; set; } = true;

    public bool HideUnderBlocks { get; set; } = false;

    public bool HideIfSneaking { get; set; } = false;

    public bool HideSpectators { get; set; } = true;
}
