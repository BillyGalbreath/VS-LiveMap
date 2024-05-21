﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using livemap.data;
using livemap.httpd;
using livemap.logger;
using livemap.network;
using livemap.network.packet;
using livemap.registry;
using livemap.task;
using livemap.task.data;
using livemap.util;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Common.Database;

namespace livemap;

[PublicAPI]
public sealed class LiveMapServer : LiveMap {
    public ICoreServerAPI Api { get; }

    public Config Config { get; private set; } = null!;

    public Colormap Colormap { get; }
    public SepiaColors SepiaColors { get; }

    public NetworkHandler NetworkHandler { get; }

    public LayerRegistry LayerRegistry { get; }
    public RendererRegistry RendererRegistry { get; }

    public JsonTaskManager JsonTaskManager { get; }
    public RenderTaskManager RenderTaskManager { get; }

    public WebServer WebServer { get; }

    public HashSet<int> MicroBlocks { get; }
    public HashSet<int> BlocksToIgnore { get; }
    public int LandBlock { get; }

    private readonly long _gameTickTaskId;

    public LiveMapServer(ICoreServerAPI api) {
        Api = api;
        LiveMap.Api = this;

        Logger.LoggerImpl = new ServerLoggerImpl();

        Files.DataDir = Path.Combine(GamePaths.DataPath, "ModData", Api.World.SavegameIdentifier, "LiveMap");
        Files.ColormapFile = Path.Combine(Files.DataDir, "colormap.json");
        Files.WebDir = Path.Combine(Files.DataDir, "web");
        Files.JsonDir = Path.Combine(Files.WebDir, "data");
        Files.MarkerDir = Path.Combine(Files.JsonDir, "markers");
        Files.TilesDir = Path.Combine(Files.WebDir, "tiles");

        ReloadConfig();

        Files.ExtractWebFiles(this);

        Colormap = new Colormap();
        SepiaColors = new SepiaColors(this);
        NetworkHandler = new ServerNetworkHandler(this);

        LayerRegistry = new LayerRegistry();
        RendererRegistry = new RendererRegistry();

        JsonTaskManager = new JsonTaskManager(this);
        RenderTaskManager = new RenderTaskManager(this);
        WebServer = new WebServer(this);

        Api.Event.ChunkDirty += OnChunkDirty;
        Api.Event.GameWorldSave += OnGameWorldSave;

        // things to do on first game tick
        Api.Event.RegisterCallback(_ => {
            Colormap.LoadFromDisk(Api.World);
            RendererRegistry.RegisterBuiltIns();
        }, 1);

        Api.ChatCommands.Create("livemap")
            .WithDescription("command.livemap.description".ToLang())
            .RequiresPrivilege("root")
            .WithArgs(new WordArgParser("command", false, new[] { "fullrender" }))
            .HandleWith(args => {
                new Thread(_ => {
                    // queue up all existing chunks
                    ImmutableList<ChunkPos> chunks = new ChunkLoader(Api).GetAllMapChunkPositions().ToImmutableList();
                    foreach (ChunkPos chunk in chunks) {
                        RenderTaskManager.Queue(chunk.X >> 4, chunk.Z >> 4);
                    }

                    // trigger world save to process the queue now
                    Api.Event.RegisterCallback(_ => {
                        // do this back on the main thread
                        Api.ChatCommands.Get("autosavenow").Execute(args);
                    }, 1);
                }).Start();
                return TextCommandResult.Success("command.fullrender.started".ToLang());
            });

        _gameTickTaskId = Api.Event.RegisterGameTickListener(OnGameTick, 1000, 1000);

        MicroBlocks = api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        BlocksToIgnore = api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                (block.Code.Path.EndsWith("-snow") && !MicroBlocks.Contains(block.Id)) ||
                block.Code.Path.EndsWith("-snow2") ||
                block.Code.Path.EndsWith("-snow3") ||
                block.Code.Path.Equals("snowblock") ||
                block.Code.Path.Contains("snowlayer-"))
            .Select(block => block.Id).ToHashSet();

        LandBlock = api.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;
    }

    public void ReloadConfig() {
        string filename = $"{LiveMapMod.Id}.json";
        Config = Api.LoadModConfig<Config>(filename) ?? new Config();
        Api.StoreModConfig(Config, filename);
    }

    private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason) {
        // queue it up, it will process when the game saves
        RenderTaskManager.Queue(chunkCoord.X >> 4, chunkCoord.Z >> 4);
    }

    private void OnGameWorldSave() {
        // delay a bit to ensure chunks actually save to disk first
        Api.Event.RegisterCallback(_ => RenderTaskManager.ProcessQueue(), 1000);
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
        Colormap.LoadFromPacket(Api.World, packet);
    }

    internal void ReceiveConfigRequest(IServerPlayer player, ConfigPacket _) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring config request packet from non-privileged user {player.PlayerName}");
            return;
        }

        Logger.Info($"&dConfig request packet was received from &n{player.PlayerName}");
        NetworkHandler.SendPacket(new ConfigPacket { Config = JsonConvert.SerializeObject(Config) }, player);
    }

    public void Dispose() {
        Api.Event.ChunkDirty -= OnChunkDirty;
        Api.Event.GameWorldSave -= OnGameWorldSave;

        Api.Event.UnregisterGameTickListener(_gameTickTaskId);

        // order matters here
        RenderTaskManager.Dispose();
        JsonTaskManager.Dispose();
        WebServer.Dispose();
        NetworkHandler.Dispose();
        LayerRegistry.Dispose();
        RendererRegistry.Dispose();
        Colormap.Dispose();
        SepiaColors.Dispose();

        RendererRegistry.Dispose();

        MicroBlocks.Clear();
        BlocksToIgnore.Clear();
    }
}
