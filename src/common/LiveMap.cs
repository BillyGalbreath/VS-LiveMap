using Vintagestory.API.Common;

namespace livemap.common;

public abstract class LiveMap(LiveMapModSystem mod) : IDisposable {
    protected readonly LiveMapModSystem _mod = mod;

    public ILogger Logger => _mod.Mod.Logger;

    public abstract void Dispose();
}
