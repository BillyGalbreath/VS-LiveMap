using System.Collections.Generic;
using livemap.common.command;
using Vintagestory.API.Common;

namespace livemap.server.command;

public sealed class ColormapCommand : ServerCommand {
    public ColormapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> _) {
        return CommandResult.Success("command.client-command");
    }
}
