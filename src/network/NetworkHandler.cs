using Vintagestory.API.Common;

namespace livemap.network;

public abstract class NetworkHandler {
    public abstract void SendPacket<T>(T packet, IPlayer? receiver = null);

    public abstract void Dispose();
}
