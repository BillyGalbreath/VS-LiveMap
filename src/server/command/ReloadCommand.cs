using System.Collections.Generic;
using System.Threading;
using livemap.common.command;
using livemap.server.configuration;
using Vintagestory.API.Common;

namespace livemap.server.command;

public sealed class ReloadCommand : ServerCommand {
    public ReloadCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("reload", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> _) {
        DoReload(Server);

        return CommandResult.Success("command.reload.finished");
    }

    public static void DoReload(LiveMapServer server) {
        // reload the main config
        Config.Reload();

        // colormap file is kinda heavy. let's load it off the main thread.
        new Thread(_ => server.Colormap.Reload(server.Api)).Start();

        // reload the web server
        server.WebServer.Reload();
    }
}
