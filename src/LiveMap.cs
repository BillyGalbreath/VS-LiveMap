using System.Linq;
using JetBrains.Annotations;
using livemap.command;
using livemap.configuration;
using livemap.data;
using livemap.httpd;
using livemap.logger;
using livemap.network;
using livemap.registry;
using livemap.task;
using livemap.task.data;
using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace livemap;

[PublicAPI]
public sealed class LiveMap {
    public static LiveMap Api { get; private set; } = null!;

    public ICoreServerAPI Sapi { get; }

    public string ModId => _mod.Mod.Info.ModID;

    public Config Config { get; private set; } = null!;

    public Colormap Colormap { get; }
    public SepiaColors SepiaColors { get; }

    public CommandHandler CommandHandler { get; }

    public LayerRegistry LayerRegistry { get; }
    public RendererRegistry RendererRegistry { get; }

    public JsonTaskManager JsonTaskManager { get; }
    public RenderTaskManager RenderTaskManager { get; }

    public WebServer WebServer { get; }

    private readonly LiveMapMod _mod;
    private readonly FileWatcher _configFileWatcher;

    private long _gameTickTaskId;
    private IServerNetworkChannel? _channel;

    public LiveMap(LiveMapMod mod, ICoreServerAPI api) {
        Api = this;

        Sapi = api;
        _mod = mod;

        Logger.LoggerImpl = new LoggerImpl(ModId, mod.Mod.Logger);

        Files.SavegameIdentifier = Sapi.World.SavegameIdentifier;
        GamePaths.EnsurePathExists(GamePaths.ModConfig);
        GamePaths.EnsurePathExists(Files.DataDir);

        _configFileWatcher = new FileWatcher(this);
        ReloadConfig();

        Files.ExtractWebFiles(this);

        Colormap = new Colormap();
        SepiaColors = new SepiaColors(this);

        CommandHandler = new CommandHandler(this);

        LayerRegistry = new LayerRegistry();
        RendererRegistry = new RendererRegistry();

        JsonTaskManager = new JsonTaskManager(this);
        RenderTaskManager = new RenderTaskManager(this);
        WebServer = new WebServer(this);

        api.Event.ChunkDirty += OnChunkDirty;
        api.Event.GameWorldSave += OnGameWorldSave;

        Start();
    }

    public void Start() {
        // things to do on first game tick
        Sapi.Event.RegisterCallback(_ => {
            Colormap.LoadFromDisk(Sapi.World);
            RendererRegistry.RegisterBuiltIns(this);
        }, 1);

        _gameTickTaskId = Sapi.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);

        _channel = Sapi.Network.RegisterChannel(ModId)
            .RegisterMessageType<ColormapPacket>()
            .SetMessageHandler<ColormapPacket>(ReceiveColormap);
    }

    public void ReloadConfig() {
        LoadConfig();
        SaveConfig();
    }

    public void LoadConfig() {
        Config = Sapi.LoadModConfig<Config>($"{ModId}.json") ?? new Config();
    }

    public void SaveConfig() {
        _configFileWatcher.IgnoreChanges = true;
        Sapi.StoreModConfig(Config, $"{ModId}.json");
        Sapi.Event.RegisterCallback(_ => _configFileWatcher.IgnoreChanges = false, 100);
    }

    public void SendPacket<T>(T packet, IPlayer? receiver = null) {
        _channel?.SendPacket(packet, receiver as IServerPlayer);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // queue it up, it will process when the game saves
        RenderTaskManager.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
    }

    private void OnGameWorldSave() {
        // delay a bit to ensure chunks actually save to disk first
        Sapi.Event.RegisterCallback(_ => RenderTaskManager.ProcessQueue(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        //RenderTask.Run();

        // ensure web server is still running
        WebServer.Run();

        // todo - update player positions, public waypoints, etc
        JsonTaskManager.Tick();
    }

    internal void ReceiveColormap(IServerPlayer player, ColormapPacket packet) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring colormap packet from non-privileged user {player.PlayerName}");
            return;
        }

        if (packet.RawColormap == null) {
            Logger.Warn($"Received null colormap from {player.PlayerName}");
            return;
        }

        Logger.Info($"&dColormap packet was received from &n{player.PlayerName}");
        Colormap.LoadFromPacket(Sapi.World, packet);
    }

    public void Dispose() {
        _configFileWatcher.Dispose();

        Sapi.Event.ChunkDirty -= OnChunkDirty;
        Sapi.Event.GameWorldSave -= OnGameWorldSave;

        Sapi.Event.UnregisterGameTickListener(_gameTickTaskId);

        CommandHandler.Dispose();

        JsonTaskManager.Dispose();
        RenderTaskManager.Dispose();

        LayerRegistry.Dispose();
        RendererRegistry.Dispose();

        Colormap.Dispose();
        SepiaColors.Dispose();

        WebServer.Dispose();

        _channel = null;
    }
}
