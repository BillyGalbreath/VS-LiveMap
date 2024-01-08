using LiveMap.Client;
using LiveMap.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LiveMap;

public sealed class LiveMapMod : ModSystem {
    public static string? Id { get; private set; }

    private LiveMapClient? client;
    private LiveMapServer? server;
    
    public override void Start(ICoreAPI api) {
        Id = Mod.Info.ModID;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        client = new LiveMapClient(this, api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        server = new LiveMapServer(this, api);
    }

    public override void Dispose() {
        client?.Dispose();
        client = null;

        server?.Dispose();
        server = null;
    }
}
