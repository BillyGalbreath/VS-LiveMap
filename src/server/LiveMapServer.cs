using System.IO;
using livemap.common;
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
    public Config Config { get; private set; } = null!;
    public Colormap Colormap { get; }
    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }

    private readonly ICoreServerAPI _api;
    private readonly RenderTask _renderTask;
    private readonly WebServer _webServer;

    private readonly long _gameTickTaskId;

    public LiveMapServer(ICoreServerAPI api) {
        _api = api;
        LiveMap.Api = this;

        Logger.LoggerImpl = new ServerLoggerImpl();

        Files.DataDir = Path.Combine(GamePaths.DataPath, "ModData", api.World.SavegameIdentifier, "LiveMap");
        Files.ColormapFile = Path.Combine(Files.DataDir, "colormap.yaml");
        Files.WebDir = Path.Combine(Files.DataDir, "web");
        Files.TilesDir = Path.Combine(Files.WebDir, "tiles");

        Files.ExtractWebFiles(api);

        ReloadConfig();

        Colormap = new Colormap();
        NetworkHandler = new ServerNetworkHandler(this, api);

        RendererRegistry = new RendererRegistry(this, api);
        RendererRegistry.RegisterBuiltIns();

        _renderTask = new RenderTask(this, api);
        _webServer = new WebServer(this);

        api.Event.ChunkDirty += OnChunkDirty;
        api.Event.GameWorldSave += OnGameWorldSave;

        api.Event.RegisterCallback(_ => Colormap.LoadFromDisk(_api), 1);

        _gameTickTaskId = api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    public void ReloadConfig() {
        string filename = $"{LiveMapMod.Id}.json";
        Config = _api.LoadModConfig<Config>(filename) ?? new Config();
        _api.StoreModConfig(Config, filename);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // queue it up, it will process when the game saves
        _renderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
    }

    private void OnGameWorldSave() {
        // delay a bit to ensure chunks actually save to disk first
        _api.Event.RegisterCallback(_ => _renderTask.ProcessQueue(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        //RenderTask.Run();

        // ensure web server is still running
        _webServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    public void Dispose() {
        _api.Event.ChunkDirty -= OnChunkDirty;
        _api.Event.GameWorldSave -= OnGameWorldSave;

        _api.Event.UnregisterGameTickListener(_gameTickTaskId);

        // order matters here
        _renderTask.Dispose();
        _webServer.Dispose();
        NetworkHandler.Dispose();
        RendererRegistry.Dispose();
        Colormap.Dispose();
    }
}
