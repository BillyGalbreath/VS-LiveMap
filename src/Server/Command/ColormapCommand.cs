using System.Collections.Generic;
using LiveMap.Common.Command;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public sealed class ColormapCommand : AbstractServerCommand {
    public ColormapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> _) {
        return CommandResult.Success("command.client-command");
    }
}
