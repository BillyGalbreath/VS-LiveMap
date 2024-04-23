using livemap.common.command;

namespace livemap.client.command;

public abstract class ClientCommand : Command {
    protected override ClientCommandHandler Handler { get; }

    protected LiveMapClient Client => Handler.Client;

    protected ClientCommand(CommandHandler handler) : base(handler) {
        Handler = (ClientCommandHandler)handler;
    }
}
