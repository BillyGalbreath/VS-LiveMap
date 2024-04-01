using LiveMap.Common.Command;
using LiveMap.Common.Extensions;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Common;

namespace LiveMap.Common;

public abstract class LiveMap {
    public virtual ICoreAPI Api { get; }

    protected abstract CommandHandler CommandHandler { get; }
    public abstract NetworkHandler NetworkHandler { get; }

    protected LiveMap(ICoreAPI api, LoggerImpl loggerImpl) {
        Api = api;

        ((Logger)null!).SetField("_loggerImpl", loggerImpl);
    }

    public virtual void Dispose() {
        NetworkHandler.Dispose();
        CommandHandler.Dispose();
    }
}
