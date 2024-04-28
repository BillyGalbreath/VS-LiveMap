using JetBrains.Annotations;
using livemap.common.configuration;
using livemap.common.network;
using livemap.common.registry;
using livemap.common.util;

namespace livemap.common;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api { get; internal set; } = null!;

    public Config Config { get; }
    public Colormap Colormap { get; }
    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }

    public void ReloadConfig();
}
