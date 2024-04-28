using livemap.server;
using Vintagestory.API.Server;

namespace livemap.common.render;

public class BasicRenderer : Renderer {
    public BasicRenderer(LiveMapServer server, ICoreServerAPI api) : base(server, api, "renderer.basic") { }
}
