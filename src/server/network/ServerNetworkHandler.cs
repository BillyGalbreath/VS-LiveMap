using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Server;

namespace livemap.server.network;

public sealed class ServerNetworkHandler : NetworkHandler {
    private readonly LiveMapServer _server;
    private readonly ICoreServerAPI _api;

    private IServerNetworkChannel? _channel;

    public ServerNetworkHandler(LiveMapServer server, ICoreServerAPI api) {
        _server = server;
        _api = api;

        _channel = api.Network.RegisterChannel(LiveMapMod.Id)
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
            if (_server.Colormap.Deserialize(_api, packet.RawColormap)) {
                _server.Colormap.SaveToDisk();
                Logger.Info("&dColormap saved to disk.");
            } else {
                Logger.Warn("Could not save colormap to disk.");
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
