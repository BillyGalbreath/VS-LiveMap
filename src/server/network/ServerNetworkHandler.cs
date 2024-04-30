using livemap.common.network;
using livemap.common.network.packet;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace livemap.server.network;

public sealed class ServerNetworkHandler : NetworkHandler {
    private IServerNetworkChannel? _channel;

    public ServerNetworkHandler(LiveMapServer server) {
        _channel = server.Api.Network.RegisterChannel(LiveMapMod.Id)
            .RegisterMessageType<ColormapPacket>()
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ColormapPacket>(server.ReceiveColormap)
            .SetMessageHandler<ConfigPacket>(server.ReceiveConfigRequest);
    }

    public override void SendPacket<T>(T packet, IPlayer? receiver = null) {
        _channel?.SendPacket(packet, receiver as IServerPlayer);
    }

    public override void Dispose() {
        _channel = null;
    }
}
