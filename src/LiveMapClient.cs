using HarmonyLib;
using livemap.configuration;
using livemap.gui;
using livemap.logger;
using livemap.network;
using livemap.network.packet;
using Newtonsoft.Json;
using Vintagestory.API.Client;

namespace livemap;

public sealed class LiveMapClient {
    private readonly Harmony _harmony;
    private readonly ConfigGui? _gui;

    public ICoreClientAPI Api { get; }

    public string ModId => _mod.Mod.Info.ModID;

    public ClientNetworkHandler NetworkHandler { get; }
    public Config? Config { get; private set; }

    private readonly LiveMapMod _mod;

    private bool _alreadyRequestedConfig;

    public LiveMapClient(LiveMapMod mod, ICoreClientAPI api) {
        Api = api;
        _mod = mod;

        Logger.LoggerImpl = new ClientLoggerImpl(mod.Mod.Info.ModID, mod.Mod.Logger);

        _harmony = new Harmony(ModId);
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
        Config = packet.Config != null ? JsonConvert.DeserializeObject<Config>(packet.Config) : null;
    }

    public void Dispose() {
        _harmony.UnpatchAll(ModId);
        _gui?.Dispose();
        NetworkHandler.Dispose();
        Config = null;
    }
}
