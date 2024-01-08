using System;
using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Network;
using LiveMap.Server.Render;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public sealed class LiveMapServer : Common.LiveMap {
    public override ICoreServerAPI Api { get; }

    public override ServerCommandHandler CommandHandler { get; }
    public override ServerNetworkHandler NetworkHandler { get; }

    public RenderTask RenderTask { get; }
    public BlockColors? BlockColors;

    private readonly long gameTickTaskId;
    private int tick;

    public LiveMapServer(LiveMapMod mod, ICoreServerAPI api) : base(mod, api) {
        Api = api;

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);
        RenderTask = new RenderTask(this);

        gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);

        Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;

        Api.Event.ServerRunPhase(EnumServerRunPhase.Shutdown, OnShutdown);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        if (tick++ > 10) {
            tick = 0;

            // todo remove this temp call
            RenderTask.Run();
        }

        // todo - update player positions, public waypoints, etc
    }

    private void OnGameWorldSave() {
        throw new NotImplementedException();
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason == EnumChunkDirtyReason.NewlyLoaded) {
            return;
        }

        int regionSize = Api.WorldManager.RegionSize;
        int chunkSize = Api.WorldManager.ChunkSize;

        int regionX = chunkCoord.X * chunkSize / regionSize;
        int regionZ = chunkCoord.Z * chunkSize / regionSize;

        RenderTask.Queue(regionX, regionZ);
    }

    public void OnShutdown() {
        RenderTask.Stop();
    }

    public void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(gameTickTaskId);

        RenderTask.Dispose();
        NetworkHandler.Dispose();
        CommandHandler.Dispose();
    }
}
