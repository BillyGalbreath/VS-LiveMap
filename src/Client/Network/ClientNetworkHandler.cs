using LiveMap.Common.Network;
using Vintagestory.API.Client;

namespace LiveMap.Client.Network;

public sealed class ClientNetworkHandler : NetworkHandler {
    private IClientNetworkChannel? channel;

    public ClientNetworkHandler(LiveMapClient client) {
        channel = client.Api.Network.RegisterChannel(LiveMapMod.Id)
            .RegisterMessageType<ColormapPacket>()
            .SetMessageHandler<ColormapPacket>(ClientReceivePacket);
    }

    private static void ClientReceivePacket(ColormapPacket packet) {
        // do nothing
    }

    public void SendPacket<T>(T packet) {
        if (channel is { Connected: true }) {
            channel.SendPacket(packet);
        }
    }

    public override void Dispose() {
        channel = null;
    }
}
