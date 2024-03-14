using System.Collections.Generic;
using LiveMap.Common.Command;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LiveMap.Server.Command;

public sealed class FullRenderCommand : AbstractServerCommand {
    public FullRenderCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("fullrender", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        // todo - queue up all existing regions
        ((IServerPlayer)caller.Player).SendMessage(CommandResult.Success("command.fullrender.started"));
        return CommandResult.Success("command.fullrender.started");
    }
}
