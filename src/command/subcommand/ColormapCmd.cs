using livemap.network;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ColormapCmd(LiveMap server) : AbstractCommand(server, ["colormap"], requiresPlayer: true) {
    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        _server.SendPacket(new ColormapPacket(), args.Caller.Player);

        return "colormap.requested".CommandSuccess();
    }
}
