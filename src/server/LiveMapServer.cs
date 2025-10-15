using livemap.common;
using livemap.common.configuration;
using Vintagestory.API.Server;

namespace livemap.server;

public sealed class LiveMapServer(LiveMapModSystem mod, ICoreServerAPI api) : LiveMap(mod) {
    private Config? _config;

    public Config Config => _config ??= api.LoadModConfig<Config>("LiveMap.json") ?? new Config();

    public override void Dispose() {
        _config = null;
    }
}
