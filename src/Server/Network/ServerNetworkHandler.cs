using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Server;

namespace LiveMap.Server.Network;

public sealed class ServerNetworkHandler : NetworkHandler {
    private readonly LiveMapServer _server;

    private IServerNetworkChannel? _channel;

    public ServerNetworkHandler(LiveMapServer server) {
        _server = server;

        _channel = server.Api.Network.RegisterChannel(LiveMapMod.Id)
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

        new Thread(_ => {
            try {
                Colormap colormap = Colormap.Deserialize(packet.RawColormap);
                Colormap.Write(colormap);
                _server.Colormap = colormap;
            } catch (Exception) {
                // ignored
            }
        }).Start();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void SendPacket<T>(T packet, IServerPlayer receiver) {
        _channel?.SendPacket(packet, receiver);
    }

    public override void Dispose() {
        _channel = null;
    }
}
