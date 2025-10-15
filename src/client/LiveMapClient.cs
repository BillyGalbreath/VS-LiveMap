using livemap.common;
using Vintagestory.API.Client;

namespace livemap.client;

public sealed class LiveMapClient(LiveMapModSystem mod, ICoreClientAPI api) : LiveMap(mod) {
    private readonly ICoreClientAPI _api = api;

    public override void Dispose() {
        // TODO release managed resources here
    }
}
