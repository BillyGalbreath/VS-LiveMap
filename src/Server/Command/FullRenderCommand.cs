using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public class FullRenderCommand : AbstractCommand {
    public FullRenderCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("fullrender", this);
    }

    public override TextCommandResult Execute(Caller caller, List<string> args) {
        return TextCommandResult.Success("fullrender-started");
    }
}
