using System.Linq;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Server;

namespace LiveMap.Server.Network;

public sealed class ServerNetworkHandler : NetworkHandler {
    private readonly LiveMapServer server;

    private IServerNetworkChannel? channel;

    public ServerNetworkHandler(LiveMapServer server) {
        this.server = server;

        channel = server.Api.Network.RegisterChannel("livemap")
            .RegisterMessageType<BlockColorsPacket>()
            .SetMessageHandler<BlockColorsPacket>(ReceivePacket);
    }

    protected override void ReceivePacket(IServerPlayer player, BlockColorsPacket packet) {
        if (!player.Privileges.Contains("livemap.admin")) {
            server.Logger.Warning("Ignoring livemap colormap packet from non-privileged user");
            return;
        }

        if (packet.RawDataColors == null) {
            server.Logger.Warning("Received null colors from client!");
            return;
        }

        server.BlockColors = BlockColors.Deserialize(packet.RawDataColors);
    }

    public override void SendPacket<T>(T packet, IServerPlayer player) {
        channel?.SendPacket(packet, player);
    }

    public override void Dispose() {
        channel = null;
    }
}
