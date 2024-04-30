using System.IO;
using System.Linq;
using livemap.common;
using livemap.common.configuration;
using livemap.common.network;
using livemap.common.network.packet;
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
    public Config Config { get; private set; } = null!;
    public Colormap Colormap { get; }
    public NetworkHandler NetworkHandler { get; }
    public RendererRegistry RendererRegistry { get; }

    private readonly RenderTask _renderTask;
    private readonly WebServer _webServer;

    private readonly long _gameTickTaskId;

    public LiveMapServer(ICoreServerAPI api) {
        Api = api;
        LiveMap.Api = this;

        Logger.LoggerImpl = new ServerLoggerImpl();

        Files.DataDir = Path.Combine(GamePaths.DataPath, "ModData", Api.World.SavegameIdentifier, "LiveMap");
        Files.ColormapFile = Path.Combine(Files.DataDir, "colormap.yaml");
        Files.WebDir = Path.Combine(Files.DataDir, "web");
        Files.TilesDir = Path.Combine(Files.WebDir, "tiles");

        ReloadConfig();

        Files.ExtractWebFiles(this);

        Colormap = new Colormap();
        NetworkHandler = new ServerNetworkHandler(this);

        RendererRegistry = new RendererRegistry(this);

        _renderTask = new RenderTask(this);
        _webServer = new WebServer(this);

        Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;

        // things to do on first game tick
        Api.Event.RegisterCallback(_ => {
            Colormap.LoadFromDisk(Api.World);
            RendererRegistry.RegisterBuiltIns();
            _renderTask.Init();
        }, 1);

        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    public void ReloadConfig() {
        string filename = $"{LiveMapMod.Id}.json";
        Config = Api.LoadModConfig<Config>(filename) ?? new Config();
        Api.StoreModConfig(Config, filename);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // queue it up, it will process when the game saves
        _renderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
    }

    private void OnGameWorldSave() {
        // delay a bit to ensure chunks actually save to disk first
        Api.Event.RegisterCallback(_ => _renderTask.ProcessQueue(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        //RenderTask.Run();

        // ensure web server is still running
        _webServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    public void ReceiveColormap(IServerPlayer player, ColormapPacket packet) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring colormap packet from non-privileged user {player.PlayerName}");
            return;
        }

        if (packet.RawColormap == null) {
            Logger.Warn($"Received null colormap from {player.PlayerName}");
            return;
        }

        Logger.Info($"&dColormap packet was received from &n{player.PlayerName}");
        Colormap.LoadFromPacket(Api.World, packet);
    }

    public void ReceiveConfigRequest(IServerPlayer player, ConfigPacket _) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring config request packet from non-privileged user {player.PlayerName}");
            return;
        }

        Logger.Info($"&dConfig request packet was received from &n{player.PlayerName}");
        NetworkHandler.SendPacket(new ConfigPacket { Config = Config }, player);
    }

    public void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        // order matters here
        _renderTask.Dispose();
        _webServer.Dispose();
        NetworkHandler.Dispose();
        RendererRegistry.Dispose();
        Colormap.Dispose();
    }
}
