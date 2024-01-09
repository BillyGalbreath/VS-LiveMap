using LiveMap.Common.Command;

namespace LiveMap.Client.Command;

public abstract class AbstractClientCommand : AbstractCommand {
    protected override ClientCommandHandler Handler { get; }

    protected AbstractClientCommand(CommandHandler handler) : base(handler) {
        Handler = (ClientCommandHandler)handler;
    }
}
