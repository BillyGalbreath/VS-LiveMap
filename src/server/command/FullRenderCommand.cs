using System.Collections.Generic;
using livemap.common.command;
using livemap.common.util;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace livemap.server.command;

public sealed class FullRenderCommand : ServerCommand {
    public FullRenderCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("fullrender", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> _) {
        if (!Server.RenderTask.ProcessAllRegions()) {
            return CommandResult.Success("command.fullrender.failed");
        }

        ((IServerPlayer)caller.Player).SendMessage(CommandResult.Success("command.fullrender.started"));
        return CommandResult.Success("command.fullrender.started");
    }
}
