using System;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class FullRenderCmd : AbstractCommand {
    public FullRenderCmd(LiveMap server) : base(server, new[] { "fullrender" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
