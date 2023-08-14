using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LiveMap.Server.Command;

public abstract class AbstractCommand {
    protected readonly CommandHandler handler;

    public AbstractCommand(CommandHandler handler) {
        this.handler = handler;
    }

    public abstract TextCommandResult Execute(Caller caller, List<string> args);
}
