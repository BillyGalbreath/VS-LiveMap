using LiveMap.Client;
using LiveMap.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LiveMap;

public class LiveMapMod : ModSystem {
    private LiveMapClient client;
    private LiveMapServer server;

    public override bool AllowRuntimeReload => true;

    public override bool ShouldLoad(EnumAppSide side) {
        return true;
    }

    public override double ExecuteOrder() {
        return 0.5;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        client = new LiveMapClient(this, api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        server = new LiveMapServer(this, api);
    }

    public override void Dispose() {
        client?.Dispose();
        server?.Dispose();
    }
}
