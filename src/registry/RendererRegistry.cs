using JetBrains.Annotations;
using livemap.render;

namespace livemap.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer> {
    public RendererRegistry() : base("renderers") { }

    public void RegisterBuiltIns(LiveMapServer server) {
        Register(new BasicRenderer(server));
        Register(new SepiaRenderer(server));
    }

    public override void Dispose() {
        foreach ((string _, Renderer renderer) in this) {
            renderer.Dispose();
        }
    }
}
