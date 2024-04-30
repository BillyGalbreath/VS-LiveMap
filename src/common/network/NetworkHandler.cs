using Vintagestory.API.Common;

namespace livemap.common.network;

public abstract class NetworkHandler {
    public abstract void SendPacket<T>(T packet, IPlayer? receiver = null);

    public abstract void Dispose();
}
