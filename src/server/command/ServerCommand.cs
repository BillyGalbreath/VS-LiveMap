using livemap.common.command;

namespace livemap.server.command;

public abstract class ServerCommand : Command {
    protected override ServerCommandHandler Handler { get; }

    protected LiveMapServer Server => Handler.Server;

    protected ServerCommand(CommandHandler handler) : base(handler) {
        Handler = (ServerCommandHandler)handler;
    }
}
