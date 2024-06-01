using JetBrains.Annotations;
using livemap.render;

namespace livemap.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer> {
    public static BasicRenderer? Basic { get; private set; }
    public static SepiaRenderer? Sepia { get; private set; }

    public RendererRegistry() : base("renderers") { }

    public void RegisterBuiltIns() {
        Register(Basic = new BasicRenderer());
        Register(Sepia = new SepiaRenderer());
    }

    public override void Dispose() {
        foreach ((string _, Renderer renderer) in this) {
            renderer.Dispose();
        }

        Basic = null;
        Sepia = null;

        base.Dispose();
    }
}
