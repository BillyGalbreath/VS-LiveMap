using JetBrains.Annotations;
using livemap.common.api.layer;

namespace livemap.common.api;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api { get; internal set; } = null!;

    public T RegisterLayer<T>(T layer) where T : Layer;

    public T UnregisterLayer<T>(T layer) where T : Layer;
}
