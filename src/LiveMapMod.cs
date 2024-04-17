using System.Diagnostics.CodeAnalysis;
using LiveMap.Client;
using LiveMap.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LiveMap;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class LiveMapMod : ModSystem {
    public static LiveMapMod Instance { get; private set; } = null!;
    public static string Id => Instance.Mod.Info.ModID;
    public static ICoreAPI Api => Instance._api!;

    private LiveMapClient? Client { get; set; }
    public LiveMapServer? Server { get; private set; }

    private ICoreAPI? _api;

    public LiveMapMod() {
        Instance = this;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _api = api;
        Client = new LiveMapClient(api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        _api = api;
        Server = new LiveMapServer(api);
    }

    public override void Dispose() {
        Client?.Dispose();
        Client = null;

        Server?.Dispose();
        Server = null;

        _api = null;
    }
}
