using JetBrains.Annotations;
using livemap.layer;

namespace livemap.registry;

[PublicAPI]
public class LayerRegistry : Registry<Layer> {
    public LayerRegistry() : base("layers") { }
}
