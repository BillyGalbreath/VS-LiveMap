using LiveMap.Common.Command;

namespace LiveMap.Server.Command;

public abstract class AbstractServerCommand : AbstractCommand {
    protected override ServerCommandHandler Handler { get; }

    protected AbstractServerCommand(CommandHandler handler) : base(handler) {
        Handler = (ServerCommandHandler)handler;
    }
}
