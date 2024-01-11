using System.Diagnostics.CodeAnalysis;
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

        channel = server.Api.Network.RegisterChannel(LiveMapMod.Id)
            .RegisterMessageType<ColormapPacket>()
            .SetMessageHandler<ColormapPacket>(ServerReceivePacket);
    }

    private void ServerReceivePacket(IServerPlayer player, ColormapPacket packet) {
        if (!player.Privileges.Contains("livemap.admin")) {
            Logger.Warn(Lang.Get("logger.warning.packet-from-non-admin", player.PlayerName));
            return;
        }

        if (packet.RawColormap == null) {
            Logger.Warn(Lang.Get("logger.warning.received-null-colormap", player.PlayerName));
            return;
        }

        Logger.Info(Lang.Get("logger.info.server-received-colormap", player.PlayerName));

        server.Colormap = Colormap.Deserialize(packet.RawColormap);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void SendPacket<T>(T packet, IServerPlayer receiver) {
        channel?.SendPacket(packet, receiver);
    }

    public override void Dispose() {
        channel = null;
    }
}
