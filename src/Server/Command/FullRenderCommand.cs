using System.Collections.Generic;
using LiveMap.Common.Command;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace LiveMap.Server.Command;

public class FullRenderCommand : AbstractServerCommand {
    public FullRenderCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("fullrender", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        ((IServerPlayer)caller.Player).SendMessage(
            GlobalConstants.GeneralChatGroup,
            $"Args: {string.Join(", ", args)}",
            EnumChatType.Notification
        );
        return CommandResult.Success("command.fullrender.started");
    }
}
