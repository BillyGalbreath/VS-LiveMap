using LiveMap.Common.Network;
using Vintagestory.API.Client;

namespace LiveMap.Client.Network;

public sealed class ClientNetworkHandler : NetworkHandler {
    private IClientNetworkChannel? channel;

    public ClientNetworkHandler(LiveMapClient client) {
        channel = client.Api.Network.RegisterChannel("livemap")
            .RegisterMessageType<BlockColorsPacket>()
            .SetMessageHandler<BlockColorsPacket>(ReceivePacket);
    }

    protected override void ReceivePacket(BlockColorsPacket packet) {
        // do nothing
    }

    public override void SendPacket<T>(T packet) {
        if (channel is { Connected: true }) {
            channel.SendPacket(packet);
        }
    }

    public override void Dispose() {
        channel = null;
    }
}
