using JetBrains.Annotations;
using livemap.common.render;
using livemap.server;
using Vintagestory.API.Server;

namespace livemap.common.registry;

[PublicAPI]
public class RendererRegistry : Registry<Renderer.Builder> {
    private readonly LiveMapServer _server;
    private readonly ICoreServerAPI _api;

    public RendererRegistry(LiveMapServer server, ICoreServerAPI api) : base("renderers") {
        _server = server;
        _api = api;
    }

    public void RegisterBuiltIns() {
        Register(new Renderer.Builder("basic", (server, api) => new BasicRenderer(server, api)));
    }

    public Renderer? Create(string id) {
        return TryGetValue(id, out Renderer.Builder? builder) ? Create(builder) : null;
    }

    public Renderer Create(Renderer.Builder builder) {
        return builder.Func.Invoke(_server, _api);
    }
}
