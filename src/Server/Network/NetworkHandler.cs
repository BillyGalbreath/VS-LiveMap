using LiveMap.Common.Network;
using LiveMap.Common.Util;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Server;

namespace LiveMap.Server.Network;

public class NetworkHandler {
    private readonly LiveMapServer server;

    private IServerNetworkChannel channel;

    public NetworkHandler(LiveMapServer server) {
        this.server = server;

        channel = server.API.Network.RegisterChannel("livemap")
            .RegisterMessageType<CanIHazColorsPacket>()
            .SetMessageHandler<CanIHazColorsPacket>(ReceivePacket);
    }

    private void ReceivePacket(IServerPlayer player, CanIHazColorsPacket packet) {
        // verify player permissions
        if (!player.Privileges.Contains("livemap.admin")) {
            server.Logger.Warning("Ignoring livemap colormap packet from non-privileged user");
            return;
        }

        // receive block colors from client
        Dictionary<string, int> colors = Colors.DeserializeColors(packet.Colors);

        // todo - log it for now
        server.Logger.Event("Colors[" + string.Join(",", colors.Select(e => e.Key + "=" + e.Value).ToArray()) + "]");
    }

    public void SendPacket<T>(T packet, IServerPlayer player) where T : Packet {
        channel.SendPacket(packet, player);
    }

    public void Dispose() {
        channel = null;
    }
}
