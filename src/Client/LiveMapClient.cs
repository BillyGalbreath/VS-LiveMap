using LiveMap.Client.Command;
using LiveMap.Client.Network;
using LiveMap.Client.Patches;
using LiveMap.Client.Util;
using Vintagestory.API.Client;

namespace LiveMap.Client;

public sealed class LiveMapClient : Common.LiveMap {
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
