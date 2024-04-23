using livemap.client.command;
using livemap.client.network;
using livemap.client.patches;
using livemap.client.util;
using livemap.common;
using Vintagestory.API.Client;

namespace livemap.client;

public sealed class LiveMapClient : LiveMapCore {
    public override ICoreClientAPI Api { get; }

    private readonly ClientHarmonyPatches _patches;

    protected override ClientCommandHandler CommandHandler { get; }
    public override ClientNetworkHandler NetworkHandler { get; }

    public LiveMapClient(ICoreClientAPI api) : base(api, new ClientLoggerImpl()) {
        Api = api;

        _patches = new ClientHarmonyPatches();
        _patches.Init();

        CommandHandler = new ClientCommandHandler(this);
        NetworkHandler = new ClientNetworkHandler(this);
    }

    public override void Dispose() {
        _patches.Dispose();
    }
}
