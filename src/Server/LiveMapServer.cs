using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Configuration;
using LiveMap.Server.Httpd;
using LiveMap.Server.Network;
using LiveMap.Server.Patches;
using LiveMap.Server.Render;
using LiveMap.Server.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public sealed class LiveMapServer : Common.LiveMap {
    public override ICoreServerAPI Api { get; }

    protected override ServerCommandHandler CommandHandler { get; }
    public override ServerNetworkHandler NetworkHandler { get; }
    public WebServer WebServer { get; }

    private readonly HarmonyPatches _patches;
    private readonly RenderTask _renderTask;
    private readonly long _gameTickTaskId;

    public Colormap? Colormap;

    public LiveMapServer(ICoreServerAPI api) : base(api) {
        Api = api;

        Config.Reload();

        FileUtil.ExtractWebFiles(Api);

        _patches = new HarmonyPatches();

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);

        _renderTask = new RenderTask(this);
        WebServer = new WebServer();

        Api.Event.ChunkDirty += OnChunkDirty;

        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        _renderTask.Run();

        // ensure web server is still running
        WebServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason != EnumChunkDirtyReason.NewlyLoaded) {
            _renderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
        }
    }

    public override void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        _renderTask.Dispose();
        WebServer.Dispose();

        base.Dispose();

        Colormap?.Dispose();

        _patches.Dispose();
    }
}
