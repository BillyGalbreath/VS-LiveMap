using JetBrains.Annotations;
using livemap.layer;
using livemap.layer.builtin;

namespace livemap.registry;

[PublicAPI]
public class LayerRegistry : Registry<Layer> {
    public LayerRegistry() : base("layers") { }

    public void RegisterBuiltIns() {
        Register(new SpawnLayer());
        Register(new PlayersLayer());
    }
}
