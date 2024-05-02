using JetBrains.Annotations;
using livemap.data;
using livemap.httpd;
using livemap.network;
using livemap.registry;
using livemap.task;

namespace livemap;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api { get; internal set; } = null!;

    public Config Config { get; }
    public Colormap Colormap { get; }
    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }
    public RenderTaskManager RenderTaskManager { get; }
    public WebServer WebServer { get; }

    public void ReloadConfig();
}
