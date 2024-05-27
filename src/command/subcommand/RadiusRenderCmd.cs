using System;
using livemap.command.argument;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ApothemRenderCmd : AbstractCommand {
    public ApothemRenderCmd(LiveMap server) : base(
        server,
        new[] { "apothemrender", "radiusrender", "rangerender" },
        requiresPlayer: true,
        argParsers: new ICommandArgumentParser[] {
            new ApothemArgParser("apothem", Math.Max(server.Sapi.WorldManager.MapSizeX, server.Sapi.WorldManager.MapSizeZ)),
            new CenterPositionArgParser("center", server.Sapi, false)
        }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
