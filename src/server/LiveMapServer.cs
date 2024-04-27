using System;
using System.Threading;
using livemap.common;
using livemap.common.api;
using livemap.common.api.layer;
using livemap.common.configuration;
using livemap.common.util;
using livemap.server.httpd;
using livemap.server.network;
using livemap.server.render;
using livemap.server.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace livemap.server;

public sealed class LiveMapServer : LiveMapCore, LiveMap {
    public override ICoreServerAPI Api { get; }

    public override ServerNetworkHandler NetworkHandler { get; }
    public WebServer WebServer { get; }

    public readonly RenderTask RenderTask;
    private readonly long _gameTickTaskId;

    private bool _firstTick;

    public Colormap Colormap = new();

    public Config Config { get; private set; } = null!;

    public LiveMapServer(ICoreServerAPI api) : base(api, new ServerLoggerImpl()) {
        Api = api;
        LiveMap.Api = this;

        ReloadConfig();

        FileUtil.ExtractWebFiles(Api);

        NetworkHandler = new ServerNetworkHandler(this);

        RenderTask = new RenderTask(this);
        WebServer = new WebServer(this);

        Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;
        Api.Event.PlayerJoin += OnPlayerJoin;

        _firstTick = true;
        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);
    }

    public void ReloadConfig() {
        string filename = $"{LiveMapMod.Id}.json";
        Config = Api.LoadModConfig<Config>(filename) ?? new Config();
        Api.StoreModConfig(Config, filename);
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

    private void OnPlayerJoin(IServerPlayer player) {
        //if (player.HasPrivilege(Privilege.root)) {
        //    NetworkHandler.SendPacket(new ConfigPacket { Config = Config }, player);
        //}
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
    }

    public T RegisterLayer<T>(T layer) where T : Layer {
        throw new NotImplementedException();
    }

    public T UnregisterLayer<T>(T layer) where T : Layer {
        throw new NotImplementedException();
    }
}
