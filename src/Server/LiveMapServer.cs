using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Network;
using LiveMap.Server.Renderer;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public class LiveMapServer {
    private readonly LiveMapMod mod;
    private readonly ICoreServerAPI api;

    private readonly CommandHandler commandHandler;
    private readonly NetworkHandler networkHandler;

    private readonly RenderTask renderTask;

    private readonly long gameTickTaskId;

    private int tick;

    public ICoreServerAPI API {
        get {
            return api;
        }
    }

    public NetworkHandler NetworkHandler {
        get {
            return networkHandler;
        }
    }

    public RenderTask RenderTask {
        get {
            return renderTask;
        }
    }

    public ILogger Logger {
        get {
            return mod.Mod.Logger;
        }
    }

    public Colors Colors;

    public LiveMapServer(LiveMapMod mod, ICoreServerAPI api) {
        this.mod = mod;
        this.api = api;

        commandHandler = new CommandHandler(this);
        networkHandler = new NetworkHandler(this);

        renderTask = new RenderTask();

        gameTickTaskId = api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);

        api.Event.ChunkDirty += OnChunkDirty;
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        if (tick++ > 10) {
            tick = 0;
            // every 10 seconds run the renderer queue
            renderTask.Run();
        }

        // todo - update player positions, etc
        //
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // todo - remove debug output
        Logger.Event($"OnDirtyChunk: {chunkCoord}");
        renderTask.RenderQueue.Enqueue(chunkCoord);
    }

    public void Dispose() {
        api.Event.ChunkDirty -= OnChunkDirty;

        api.Event.UnregisterGameTickListener(gameTickTaskId);

        renderTask.Dispose();

        networkHandler.Dispose();
        commandHandler.Dispose();
    }
}
