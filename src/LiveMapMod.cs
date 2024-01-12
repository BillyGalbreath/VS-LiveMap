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
    public static ICoreAPI Api => Instance.api!;

    private LiveMapClient? Client { get; set; }
    private LiveMapServer? Server { get; set; }

    private ICoreAPI? api;

    public LiveMapMod() {
        Instance = this;
    }

    public override void StartClientSide(ICoreClientAPI capi) {
        api = capi;
        Client = new LiveMapClient(capi);
    }

    public override void StartServerSide(ICoreServerAPI sapi) {
        api = sapi;
        Server = new LiveMapServer(sapi);
    }

    public override void Dispose() {
        Client?.Dispose();
        Client = null;

        Server?.Dispose();
        Server = null;

        api = null;
    }
}
