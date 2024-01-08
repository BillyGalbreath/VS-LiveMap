using LiveMap.Common.Command;
using LiveMap.Common.Network;
using Vintagestory.API.Common;

namespace LiveMap.Common;

public abstract class LiveMap {
    public virtual ICoreAPI Api { get; }
    public LiveMapMod Mod { get; }

    public ILogger Logger => Mod.Mod.Logger;
    
    public abstract CommandHandler CommandHandler { get; }
    public abstract NetworkHandler NetworkHandler { get; }

    protected LiveMap(LiveMapMod mod, ICoreAPI api) {
        Api = api;
        Mod = mod;
    }
}
