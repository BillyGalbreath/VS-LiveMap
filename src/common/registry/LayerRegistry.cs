using JetBrains.Annotations;
using livemap.common.api.layer;

namespace livemap.common.registry;

[PublicAPI]
public class LayerRegistry : Registry<Layer> {
    public LayerRegistry() : base("layers") { }
}
