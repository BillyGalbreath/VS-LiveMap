using JetBrains.Annotations;
using livemap.render;

namespace livemap.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer> {
    public RendererRegistry() : base("renderers") { }

    public void RegisterBuiltIns() {
        Register(new BasicRenderer());
        Register(new SepiaRenderer());
    }

    public override void Dispose() {
        foreach ((string _, Renderer renderer) in this) {
            renderer.Dispose();
        }
    }
}
