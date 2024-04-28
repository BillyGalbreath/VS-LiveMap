using JetBrains.Annotations;
using livemap.common.layer;

namespace livemap.common.registry;

[PublicAPI]
public class LayerRegistry : Registry<Layer> {
    public LayerRegistry() : base("layers") { }
}
