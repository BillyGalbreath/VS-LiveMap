using livemap.network.packet;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace livemap.network;

public sealed class ClientNetworkHandler : NetworkHandler {
    private IClientNetworkChannel? _channel;

    public ClientNetworkHandler(LiveMapClient client) {
        _channel = client.Api.Network.RegisterChannel(client.ModId)
            .RegisterMessageType<ColormapPacket>()
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ColormapPacket>(_ => { })
            .SetMessageHandler<ConfigPacket>(client.ReceiveConfig);
    }

    public override void SendPacket<T>(T packet, IPlayer? unused = null) {
        if (_channel is { Connected: true }) {
            _channel.SendPacket(packet);
        }
    }

    public override void Dispose() {
        _channel = null;
    }
}
