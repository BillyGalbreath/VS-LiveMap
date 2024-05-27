using System;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ReloadCmd : AbstractCommand {
    public ReloadCmd(LiveMap server) : base(server, new[] { "reload" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
