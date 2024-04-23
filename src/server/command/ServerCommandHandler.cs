using livemap.common.command;
using livemap.common.util;
using Vintagestory.API.Common;
using Lang = livemap.common.util.Lang;

namespace livemap.server.command;

public sealed class ServerCommandHandler : CommandHandler {
    internal LiveMapServer Server { get; }

    public ServerCommandHandler(LiveMapServer server) : base(server) {
        Server = server;

        Server.Api.Permissions.RegisterPrivilege("root", Lang.Get("command.livemap.description"), false);
    }

    protected override void RegisterSubCommands() {
        _ = new ColormapCommand(this);
        _ = new FullRenderCommand(this);
        _ = new ReloadCommand(this);
    }

    protected override TextCommandResult VanillaExecute(TextCommandCallingArgs args) {
        CommandResult result = InternalExecute(args);

        if (result.Message.Length != 0) {
            args.Caller.SendMessage(result);
        }

        return TextCommandResult.Deferred;
    }
}
