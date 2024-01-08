using LiveMap.Common.Command;

namespace LiveMap.Client.Command;

public abstract class AbstractClientCommand : AbstractCommand {
    protected ClientCommandHandler ClientHandler => (ClientCommandHandler)Handler;

    protected AbstractClientCommand(CommandHandler handler) : base(handler) {
    }

    protected void Send(string message) {
        ClientHandler.Client.Api.ShowChatMessage(message);
    }
}
