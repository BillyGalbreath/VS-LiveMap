using System.Reflection;
using HarmonyLib;
using livemap.client.gui;
using livemap.client.network;
using livemap.client.util;
using livemap.common;
using livemap.common.configuration;
using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Client;
using Vintagestory.Common;

namespace livemap.client;

public sealed class LiveMapClient : LiveMapCore {
    public override ICoreClientAPI Api { get; }

    private readonly Harmony _harmony;
    private readonly ConfigGui? _gui;

    public override ClientNetworkHandler NetworkHandler { get; }

    public Config? Config { get; private set; }

    public LiveMapClient(ICoreClientAPI api) : base(api, new ClientLoggerImpl()) {
        Api = api;

        _harmony = new Harmony(LiveMapMod.Id);
        _harmony.Patch(typeof(GameCalendar).GetProperty("YearRel", BindingFlags.Instance | BindingFlags.Public)!.GetGetMethod(),
            prefix: typeof(ColormapGenerator).GetMethod("PreYearRel"));

        if (Api.ModLoader.IsModEnabled("configlib")) {
            Logger.Info("Found ConfigLib. Registering gui for settings.");
            _gui = new ConfigGui(this);
        }

        NetworkHandler = new ClientNetworkHandler(this);
    }

    public void ReceiveConfig(ConfigPacket packet) {
        Logger.Debug("Received config packet from server");
        Config = packet.Config!;

        Logger.Debug($"TileType: {Config.Web.TileType}");
    }

    public override void Dispose() {
        _harmony.UnpatchAll(LiveMapMod.Id);

        _gui?.Dispose();
    }
}
