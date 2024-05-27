using System;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class StatusCmd : AbstractCommand {
    public StatusCmd(LiveMap server) : base(server, new[] { "status" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
