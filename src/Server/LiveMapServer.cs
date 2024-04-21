using System;
using System.Threading;
using LiveMap.Common.Api;
using LiveMap.Common.Api.Layer.Marker;
using LiveMap.Common.Api.Layer.Options;
using LiveMap.Common.Api.Layer.Options.Marker;
using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Configuration;
using LiveMap.Server.Httpd;
using LiveMap.Server.Network;
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

    public readonly RenderTask RenderTask;
    private readonly long _gameTickTaskId;

    private bool _firstTick;

    public Colormap Colormap = new();

    public LiveMapServer(ICoreServerAPI api) : base(api, new ServerLoggerImpl()) {
        Api = api;

        Config.Reload();

        FileUtil.ExtractWebFiles(Api);

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);

        RenderTask = new RenderTask(this);
        WebServer = new WebServer();

        Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;

        _firstTick = true;
        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason != EnumChunkDirtyReason.NewlyLoaded) {
            RenderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
        }
    }

    private void OnGameWorldSave() {
        // delay to ensure chunks actually save to disk first
        Api.Event.RegisterCallback(_ => RenderTask.ProcessQueue(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // first game tick tasks
        if (_firstTick) {
            _firstTick = false;

            // colormap file is kinda heavy. let's load it off the main thread.
            new Thread(_ => Colormap.Reload(Api)).Start();
        }

        // ensure render task is running
        //_renderTask.Run();

        // ensure web server is still running
        WebServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    public override void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        RenderTask.Dispose();
        WebServer.Dispose();

        base.Dispose();

        Colormap.Dispose();

        Config.Dispose();
    }
}
