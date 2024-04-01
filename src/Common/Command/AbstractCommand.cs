using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LiveMap.Common.Command;

public abstract class AbstractCommand {
    protected virtual CommandHandler Handler { get; }

    protected AbstractCommand(CommandHandler handler) {
        Handler = handler;
    }

    public abstract CommandResult Execute(Caller caller, IEnumerable<string> _);
}
