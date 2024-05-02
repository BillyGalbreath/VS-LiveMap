using JetBrains.Annotations;
using livemap.render;

namespace livemap.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer.Builder> {
    private readonly LiveMapServer _server;

    public RendererRegistry(LiveMapServer server) : base("renderers") {
        _server = server;
    }

    public override bool Register(string id, Renderer.Builder builder) {
        _server.RenderTaskManager.RenderTask.Renderers.Add(id, builder.Func.Invoke(_server));
        return base.Register(id, builder);
    }

    public void RegisterBuiltIns() {
        Register(new Renderer.Builder("basic", server => new BasicRenderer(server)));
        Register(new Renderer.Builder("medieval", server => new MedievalRenderer(server)));
    }

    public Renderer? Create(string id) {
        return TryGetValue(id, out Renderer.Builder? builder) ? Create(builder) : null;
    }

    public Renderer Create(Renderer.Builder builder) {
        return builder.Func.Invoke(_server);
    }
}
