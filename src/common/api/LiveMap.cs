using JetBrains.Annotations;
using livemap.common.network;
using livemap.common.registry;

namespace livemap.common.api;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api { get; internal set; } = null!;

    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }
}
