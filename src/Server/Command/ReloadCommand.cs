using System.Collections.Generic;
using LiveMap.Common.Command;
using LiveMap.Server.Configuration;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public sealed class ReloadCommand : AbstractServerCommand {
    public ReloadCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("reload", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        Config.Reload();
        Server.WebServer.Reload();
        return CommandResult.Success("command.reload.finished");
    }
}
