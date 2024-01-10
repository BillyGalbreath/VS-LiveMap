using LiveMap.Common.Util;
using LiveMap.Server.Command;
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

    public Colormap? Colormap;

    private readonly HarmonyPatches patches;
    private readonly RenderTask renderTask;
    private readonly long gameTickTaskId;

    public LiveMapServer(LiveMapMod mod, ICoreServerAPI api) : base(mod, api) {
        Api = api;

        patches = new HarmonyPatches(mod);

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);

        renderTask = new RenderTask(this);

        gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);

        Api.Event.ChunkDirty += OnChunkDirty;

        Api.Event.ServerRunPhase(EnumServerRunPhase.Shutdown, OnShutdown);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        Logger.Debug("---- OnGameTick");

        // ensure render task is running
        renderTask.Run();

        // todo - update player positions, public waypoints, etc
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason == EnumChunkDirtyReason.NewlyLoaded) {
            return;
        }

        int regionX = chunkCoord.X << 5 >> 9;
        int regionZ = chunkCoord.Z << 5 >> 9;

        renderTask.Queue(regionX, regionZ);
    }

    private void OnShutdown() {
        Logger.Debug("---- OnShutdown");
        renderTask.Stop();
    }

    public override void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;

        Api.Event.UnregisterGameTickListener(gameTickTaskId);

        renderTask.Dispose();

        base.Dispose();

        Colormap?.Dispose();

        patches.Dispose();
    }
}
