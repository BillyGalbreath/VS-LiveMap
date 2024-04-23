using System.Collections.Generic;
using Vintagestory.API.Common;

namespace livemap.common.command;

public abstract class Command {
    protected virtual CommandHandler Handler { get; }

    protected Command(CommandHandler handler) {
        Handler = handler;
    }

    public abstract CommandResult Execute(Caller caller, IEnumerable<string> _);
}
