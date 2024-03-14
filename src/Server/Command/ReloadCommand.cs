using System.Collections.Generic;
using System.Threading;
using LiveMap.Common.Command;
using LiveMap.Common.Configuration;
using LiveMap.Common.Util;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public sealed class ReloadCommand : AbstractServerCommand {
    public ReloadCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("reload", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        // reload the main config
        Config.Reload();

        // colormap file is kinda heavy. lets load it off the main thread.
        new Thread(_ => Server.Colormap = Colormap.Read()).Start();

        // reload the web server
        Server.WebServer.Reload();

        return CommandResult.Success("command.reload.finished");
    }
}
