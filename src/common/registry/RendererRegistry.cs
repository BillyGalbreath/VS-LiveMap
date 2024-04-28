using JetBrains.Annotations;
using livemap.common.render;
using livemap.server;

namespace livemap.common.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer.Builder> {
    private readonly LiveMapServer _server;

    public RendererRegistry(LiveMapServer server) : base("renderers") {
        _server = server;
    }

    public void RegisterBuiltIns() {
        Register(new Renderer.Builder("basic", server => new BasicRenderer(server)));
    }

    public Renderer? Create(string id) {
        return TryGetValue(id, out Renderer.Builder? builder) ? Create(builder) : null;
    }

    public Renderer Create(Renderer.Builder builder) {
        return builder.Func.Invoke(_server);
    }
}
