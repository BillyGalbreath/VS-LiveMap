using LiveMap.Common.Command;
using LiveMap.Common.Network;
using Vintagestory.API.Common;

namespace LiveMap.Common;

public abstract class LiveMap {
    public virtual ICoreAPI Api { get; }
    private LiveMapMod Mod { get; }

    protected abstract CommandHandler CommandHandler { get; }
    public abstract NetworkHandler NetworkHandler { get; }

    protected LiveMap(LiveMapMod mod, ICoreAPI api) {
        Api = api;
        Mod = mod;
    }

    public virtual void Dispose() {
        NetworkHandler.Dispose();
        CommandHandler.Dispose();
    }
}
