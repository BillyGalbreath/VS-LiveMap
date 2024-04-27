using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Server;

namespace livemap.server.network;

public sealed class ServerNetworkHandler : NetworkHandler {
    private readonly LiveMapServer _server;

    private IServerNetworkChannel? _channel;

    public ServerNetworkHandler(LiveMapServer server) {
        _server = server;

        _channel = server.Api.Network.RegisterChannel(LiveMapMod.Id)
            .RegisterMessageType<ColormapPacket>()
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ColormapPacket>(ReceiveColormap)
            .SetMessageHandler<ConfigPacket>(ReceiveConfigRequest);
    }

    private void ReceiveColormap(IServerPlayer player, ColormapPacket packet) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring colormap packet from non-privileged user {player.PlayerName}");
            return;
        }

        if (packet.RawColormap == null) {
            Logger.Warn($"Received null colormap from {player.PlayerName}");
            return;
        }

        Logger.Info($"&dColormap packet was received from &n{player.PlayerName}");

        new Thread(_ => {
            try {
                Colormap colormap = Colormap.Deserialize(packet.RawColormap);
                colormap.RefreshIds(player.Entity.World);
                colormap.Write();
                _server.Colormap = colormap;
            } catch (Exception) {
                // ignored
            }
        }).Start();
    }

    private void ReceiveConfigRequest(IServerPlayer player, ConfigPacket packet) {
        if (!player.Privileges.Contains("root")) {
            Logger.Warn($"Ignoring config request packet from non-privileged user {player.PlayerName}");
            return;
        }

        Logger.Info($"&dConfig request packet was received from &n{player.PlayerName}");
        SendPacket(new ConfigPacket { Config = _server.Config }, player);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public void SendPacket<T>(T packet, IServerPlayer receiver) {
        _channel?.SendPacket(packet, receiver);
    }

    public override void Dispose() {
        _channel = null;
    }
}
