using System.Collections.Generic;
using JetBrains.Annotations;
using livemap.data;
using livemap.httpd;
using livemap.network;
using livemap.registry;
using livemap.task;
using livemap.task.data;

namespace livemap;

[PublicAPI]
public interface LiveMap {
    public static LiveMap Api { get; internal set; } = null!;

    public Config Config { get; }

    public Colormap Colormap { get; }
    public SepiaColors SepiaColors { get; }

    public NetworkHandler NetworkHandler { get; }

    public LayerRegistry LayerRegistry { get; }
    public RendererRegistry RendererRegistry { get; }

    public JsonTaskManager JsonTaskManager { get; }
    public RenderTaskManager RenderTaskManager { get; }

    public WebServer WebServer { get; }

    public HashSet<int> MicroBlocks { get; }
    public HashSet<int> BlocksToIgnore { get; }
    public int LandBlock { get; }

    public void ReloadConfig();
}
