using HarmonyLib;
using livemap.data;
using livemap.gui;
using livemap.logger;
using livemap.network;
using livemap.network.packet;
using Vintagestory.API.Client;

namespace livemap;

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

    internal void RequestConfig() {
        if (_alreadyRequestedConfig) {
            return;
        }
        Logger.Debug("Requesting updated config from server");
        NetworkHandler.SendPacket(new ConfigPacket());
        _alreadyRequestedConfig = true;
    }

    internal void ReceiveConfig(ConfigPacket packet) {
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
