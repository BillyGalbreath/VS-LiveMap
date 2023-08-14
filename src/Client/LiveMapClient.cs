using LiveMap.Client.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace LiveMap.Client;

public class LiveMapClient {
    private readonly LiveMapMod mod;
    private readonly ICoreClientAPI api;

    private readonly NetworkHandler networkHandler;

    public ICoreClientAPI API { get { return api; } }
    public ILogger Logger { get { return mod.Mod.Logger; } }

    public LiveMapClient(LiveMapMod mod, ICoreClientAPI api) {
        this.mod = mod;
        this.api = api;

        networkHandler = new NetworkHandler(this);
    }

    public void Dispose() {
        networkHandler.Dispose();
    }
}
