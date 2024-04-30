using HarmonyLib;
using livemap.client.gui;
using livemap.client.network;
using livemap.client.util;
using livemap.common.configuration;
using livemap.common.network.packet;
using livemap.common.util;
using Vintagestory.API.Client;

namespace livemap.client;

public sealed class LiveMapClient {
    private readonly Harmony _harmony;
    private readonly ConfigGui? _gui;

    public ICoreClientAPI Api { get; }
    public ClientNetworkHandler NetworkHandler { get; }
    public Config? Config { get; private set; }

    private bool _alreadyRequestedConfig;

    public LiveMapClient(ICoreClientAPI api) {
        Api = api;
        Logger.LoggerImpl = new ClientLoggerImpl();

        _harmony = new Harmony(LiveMapMod.Id);
        _harmony.PatchAll();

        if (Api.ModLoader.IsModEnabled("configlib")) {
            Logger.Info("Found ConfigLib. Registering gui for settings.");
            _gui = new ConfigGui(this);
        }

        NetworkHandler = new ClientNetworkHandler(this);
    }

    public void RequestConfig() {
        if (_alreadyRequestedConfig) {
            return;
        }
        Logger.Debug("Requesting updated config from server");
        NetworkHandler.SendPacket(new ConfigPacket());
        _alreadyRequestedConfig = true;
    }

    public void ReceiveConfig(ConfigPacket packet) {
        Logger.Debug("Received config packet from server");
        Config = packet.Config;
    }

    public void Dispose() {
        _harmony.UnpatchAll(LiveMapMod.Id);
        _gui?.Dispose();
        NetworkHandler.Dispose();
        Config = null;
    }
}
