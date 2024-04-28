using System;
using System.IO;
using livemap.common.api;
using livemap.common.configuration;
using livemap.common.network;
using livemap.common.registry;
using livemap.common.util;
using livemap.server.httpd;
using livemap.server.network;
using livemap.server.task;
using livemap.server.util;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace livemap.server;

public sealed class LiveMapServer : LiveMap {
    public ICoreServerAPI Api { get; }

    public Colormap Colormap { get; }
    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }
    public RenderTask RenderTask { get; }
    public WebServer WebServer { get; }

    public Config Config { get; private set; } = null!;

    private readonly long _gameTickTaskId;

    public LiveMapServer(ICoreServerAPI api) {
        Api = api;
        LiveMap.Api = this;

        Logger.LoggerImpl = new ServerLoggerImpl();

        Files.DataDir = Path.Combine(GamePaths.DataPath, "ModData", api.World.SavegameIdentifier, "LiveMap");
        Files.ColormapFile = Path.Combine(Files.DataDir, "colormap.yaml");
        Files.WebDir = Path.Combine(Files.DataDir, "web");
        Files.TilesDir = Path.Combine(Files.WebDir, "tiles");

        Files.ExtractWebFiles(api);

        ReloadConfig();

        Colormap = new Colormap();
        NetworkHandler = new ServerNetworkHandler(this);

        RendererRegistry = new RendererRegistry(this);
        RendererRegistry.RegisterBuiltIns();

        RenderTask = new RenderTask(this);
        WebServer = new WebServer(this);

        api.Event.ChunkDirty += OnChunkDirty;
        api.Event.GameWorldSave += OnGameWorldSave;

        api.Event.RegisterCallback(_ => Colormap.LoadFromDisk(Api), 1);

        _gameTickTaskId = api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    public void ReloadConfig() {
        string filename = $"{LiveMapMod.Id}.json";
        Config = Api.LoadModConfig<Config>(filename) ?? new Config();
        Api.StoreModConfig(Config, filename);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // queue it up, it will process when the game saves
        RenderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
    }

    private void OnGameWorldSave() {
        // delay a bit to ensure chunks actually save to disk first
        Api.Event.RegisterCallback(_ => RenderTask.ProcessQueue(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        //RenderTask.Run();

        // ensure web server is still running
        WebServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    public void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        // order matters here
        RenderTask.Dispose();
        WebServer.Dispose();
        NetworkHandler.Dispose();
        RendererRegistry.Dispose();
        Colormap.Dispose();
    }
}
