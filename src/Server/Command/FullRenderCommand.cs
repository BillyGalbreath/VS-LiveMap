using System.Collections.Generic;
using LiveMap.Common.Command;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public class FullRenderCommand : AbstractCommand {
    public FullRenderCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("fullrender", this);
    }

    public override CommandResult Execute(Caller caller, List<string> args) {
        return Success("fullrender-started");
    }
}
