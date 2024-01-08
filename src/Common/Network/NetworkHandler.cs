using System;
using Vintagestory.API.Server;

namespace LiveMap.Common.Network;

public abstract class NetworkHandler {
    protected virtual void ReceivePacket(BlockColorsPacket packet) {
        throw new NotImplementedException();
    }

    protected virtual void ReceivePacket(IServerPlayer player, BlockColorsPacket packet) {
        throw new NotImplementedException();
    }

    public virtual void SendPacket<T>(T packet) where T : Packet {
        throw new NotImplementedException();
    }

    public virtual void SendPacket<T>(T packet, IServerPlayer player) where T : Packet {
        throw new NotImplementedException();
    }

    public abstract void Dispose();
}
