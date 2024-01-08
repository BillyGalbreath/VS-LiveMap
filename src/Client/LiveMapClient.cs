using LiveMap.Client.Command;
using LiveMap.Client.Network;
using Vintagestory.API.Client;

namespace LiveMap.Client;

public sealed class LiveMapClient : Common.LiveMap {
    public override ICoreClientAPI Api { get; }

    public override ClientCommandHandler CommandHandler { get; }
    public override ClientNetworkHandler NetworkHandler { get; }

    public LiveMapClient(LiveMapMod mod, ICoreClientAPI api) : base(mod, api) {
        Api = api;

        CommandHandler = new ClientCommandHandler(this);
        NetworkHandler = new ClientNetworkHandler(this);
    }

    public void Dispose() {
        NetworkHandler.Dispose();
    }
}
