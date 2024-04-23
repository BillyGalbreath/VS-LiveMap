using JetBrains.Annotations;
using livemap.common.api.layer;

namespace livemap;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api => LiveMapMod.Instance.Server!;

    public T RegisterLayer<T>(T layer) where T : Layer;

    public T UnregisterLayer<T>(T layer) where T : Layer;
}
