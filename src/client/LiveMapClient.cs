using System.Reflection;
using HarmonyLib;
using livemap.client.gui;
using livemap.client.network;
using livemap.client.util;
using livemap.common.configuration;
using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Client;
using Vintagestory.Common;

namespace livemap.client;

public sealed class LiveMapClient {
    public ICoreClientAPI Api { get; }

    private readonly Harmony _harmony;
    private readonly ConfigGui? _gui;

    public ClientNetworkHandler NetworkHandler { get; }

    public Config? Config { get; private set; }

    public LiveMapClient(ICoreClientAPI api) {
        Api = api;

        Logger.LoggerImpl = new ClientLoggerImpl();

        _harmony = new Harmony(LiveMapMod.Id);
        _harmony.Patch(typeof(GameCalendar).GetProperty("YearRel", BindingFlags.Instance | BindingFlags.Public)!.GetGetMethod(),
            prefix: typeof(ColormapGenerator).GetMethod("PreYearRel"));

        if (api.ModLoader.IsModEnabled("configlib")) {
            Logger.Info("Found ConfigLib. Registering gui for settings.");
            _gui = new ConfigGui(this);
        }

        NetworkHandler = new ClientNetworkHandler(this);
    }

    public void ReceiveConfig(ConfigPacket packet) {
        Logger.Debug("Received config packet from server");
        Config = packet.Config!;
    }

    public void Dispose() {
        _harmony.UnpatchAll(LiveMapMod.Id);

        _gui?.Dispose();

        Config = null;
    }
}
