using LiveMap.Server.Command;
using LiveMap.Server.Network;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LiveMap.Server;

public class LiveMapServer {
    private readonly LiveMapMod mod;
    private readonly ICoreServerAPI api;

    private readonly CommandHandler commandHandler;
    private readonly NetworkHandler networkHandler;

    public ICoreServerAPI API { get { return api; } }
    public NetworkHandler NetworkHandler { get { return networkHandler; } }
    public ILogger Logger { get { return mod.Mod.Logger; } }

    public LiveMapServer(LiveMapMod mod, ICoreServerAPI api) {
        this.mod = mod;
        this.api = api;

        commandHandler = new CommandHandler(this);
        networkHandler = new NetworkHandler(this);
    }

    public void Dispose() {
        commandHandler.Dispose();
        networkHandler.Dispose();
    }
}
