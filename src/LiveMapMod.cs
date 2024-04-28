using System.Diagnostics.CodeAnalysis;
using livemap.client;
using livemap.server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace livemap;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class LiveMapMod : ModSystem {
    private static LiveMapMod? _instance;

    public static string Id => _instance!.Mod.Info.ModID;
    public static ILogger VanillaLogger => _instance!.Mod.Logger;

    private LiveMapClient? Client { get; set; }
    public LiveMapServer? Server { get; private set; }


    // todo remove all this api stuff
    public static ICoreAPI Api => _instance!._api!;
    private ICoreAPI? _api;
    // todo end

    public LiveMapMod() {
        _instance = this;
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
