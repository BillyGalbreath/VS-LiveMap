﻿using System.Threading;
using LiveMap.Common.Configuration;
using LiveMap.Common.Util;
using LiveMap.Server.Command;
using LiveMap.Server.Httpd;
using LiveMap.Server.Network;
using LiveMap.Server.Patches;
using LiveMap.Server.Render;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public sealed class LiveMapServer : Common.LiveMap {
    public override ICoreServerAPI Api { get; }

    protected override ServerCommandHandler CommandHandler { get; }
    public override ServerNetworkHandler NetworkHandler { get; }
    public WebServer WebServer { get; }

    private readonly ServerHarmonyPatches _patches;
    private readonly RenderTask _renderTask;
    private readonly long _gameTickTaskId;

    private bool _firstTick;

    public Colormap? Colormap;

    public LiveMapServer(ICoreServerAPI api) : base(api) {
        Api = api;

        Config.Reload();

        FileUtil.ExtractWebFiles(Api);

        _patches = new ServerHarmonyPatches();
        _patches.Init();

        CommandHandler = new ServerCommandHandler(this);
        NetworkHandler = new ServerNetworkHandler(this);

        _renderTask = new RenderTask(this);
        WebServer = new WebServer();

        //Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;

        _firstTick = true;
        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    private void OnGameWorldSave() {
        Api.Event.RegisterCallback(_ => _renderTask.Run(), 1000);
    }

    // this method ticks every 1000ms on the game thread
    private void OnGameTick(float delta) {
        // first game tick tasks
        if (_firstTick) {
            _firstTick = false;

            // colormap file is kinda heavy. lets load it off the main thread.
            new Thread(_ => Colormap = Colormap.Read()).Start();
        }

        // ensure render task is running
        //_renderTask.Run();

        // ensure web server is still running
        WebServer.Run();

        // todo - update player positions, public waypoints, etc
    }

    /*private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        if (reason != EnumChunkDirtyReason.NewlyLoaded) {
            _renderTask.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
        }
    }*/

    public override void Dispose() {
        //Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        _renderTask.Dispose();
        WebServer.Dispose();

        base.Dispose();

        Colormap?.Dispose();

        _patches.Dispose();
    }
}
