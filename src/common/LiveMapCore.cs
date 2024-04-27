using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Common;

namespace livemap.common;

public abstract class LiveMapCore {
    public virtual ICoreAPI Api { get; }

    public abstract NetworkHandler NetworkHandler { get; }

    protected LiveMapCore(ICoreAPI api, LoggerImpl loggerImpl) {
        Api = api;

        Logger.LoggerImpl = loggerImpl;
    }

    public virtual void Dispose() {
        NetworkHandler.Dispose();
    }
}
