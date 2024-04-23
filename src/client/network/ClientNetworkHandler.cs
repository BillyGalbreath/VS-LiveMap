using livemap.common.network;
using Vintagestory.API.Client;

namespace livemap.client.network;

public sealed class ClientNetworkHandler : NetworkHandler {
    private IClientNetworkChannel? _channel;

    public ClientNetworkHandler(LiveMapClient client) {
        _channel = client.Api.Network.RegisterChannel(LiveMapMod.Id)
            .RegisterMessageType<ColormapPacket>()
            .SetMessageHandler<ColormapPacket>(ClientReceivePacket);
    }

    private static void ClientReceivePacket(ColormapPacket packet) {
        // do nothing
    }

    public void SendPacket<T>(T packet) {
        if (_channel is { Connected: true }) {
            _channel.SendPacket(packet);
        }
    }

    public override void Dispose() {
        _channel = null;
    }
}
