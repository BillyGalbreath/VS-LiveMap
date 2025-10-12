using livemap.render;

namespace livemap.registry;

public class RendererRegistry() : Registry<Renderer>("renderers") {
    public static BasicRenderer? Basic { get; private set; }
    public static SepiaRenderer? Sepia { get; private set; }

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
