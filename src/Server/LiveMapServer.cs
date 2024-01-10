using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Httpd;
using LiveMap.Server.Network;
using LiveMap.Server.Patches;
using LiveMap.Server.Render;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public sealed class LiveMapServer : Common.LiveMap {
    public override ICoreServerAPI Api { get; }

    protected override ServerCommandHandler CommandHandler { get; }
    public override ServerNetworkHandler NetworkHandler { get; }

    private readonly HarmonyPatches patches;
    private readonly RenderTask renderTask;
    private readonly WebServer webServer;
    private readonly long gameTickTaskId;

    public Colormap? Colormap;

    public LiveMapServer(LiveMapMod mod, ICoreServerAPI api) : base(mod, api) {
        Api = api;

        patches = new HarmonyPatches(mod);

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);

        renderTask = new RenderTask(this);
        webServer = new WebServer();

        Api.Event.ChunkDirty += OnChunkDirty;

        gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // ensure render task is running
        renderTask.Run();

        // ensure web server is still running
        webServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason != EnumChunkDirtyReason.NewlyLoaded) {
            renderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
        }
    }

    public override void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;

        Api.Event.UnregisterGameTickListener(gameTickTaskId);

        renderTask.Dispose();
        webServer.Dispose();

        base.Dispose();

        Colormap?.Dispose();

        patches.Dispose();
    }
}
