using livemap.network;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ColormapCmd : AbstractCommand {
    public ColormapCmd(LiveMap server) : base(
        server,
        new[] { "colormap" },
        requiresPlayer: true
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        _server.SendPacket(new ColormapPacket(), args.Caller.Player);

        return "colormap.requested".CommandSuccess();
    }
}
