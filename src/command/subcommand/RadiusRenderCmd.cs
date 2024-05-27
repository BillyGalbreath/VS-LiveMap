using System;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class RadiusRenderCmd : AbstractCommand {
    public RadiusRenderCmd(LiveMap server) : base(server, new[] { "radiusrender" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
