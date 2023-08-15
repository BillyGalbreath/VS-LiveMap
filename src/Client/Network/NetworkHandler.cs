using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Client;

namespace LiveMap.Client.Network;

public class NetworkHandler {
    private readonly LiveMapClient client;

    private IClientNetworkChannel channel;

    public NetworkHandler(LiveMapClient client) {
        this.client = client;

        channel = client.API.Network
            .RegisterChannel("livemap")
            .RegisterMessageType<CanIHazColorsPacket>()
            .SetMessageHandler<CanIHazColorsPacket>(ReceivePacket);
    }

    private void ReceivePacket(CanIHazColorsPacket packet) {
        // server wants our block colors
        packet.Colors = Colors.Serialize(client.API);

        // send back to server
        SendPacket(packet);
    }

    public void SendPacket<T>(T packet) where T : Packet {
        channel.SendPacket(packet);
    }

    public void Dispose() {
        channel = null;
    }
}
