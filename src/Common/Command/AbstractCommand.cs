using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LiveMap.Common.Command;

public abstract class AbstractCommand {
    protected virtual CommandHandler Handler { get; }

    protected AbstractCommand(CommandHandler handler) {
        Handler = handler;
    }

    public abstract CommandResult Execute(Caller caller, IEnumerable<string> args);
}

public class CommandResult {
    public required EnumCommandStatus Status;
    public required string Message;
    public object[]? Args;

    public static CommandResult Success(string message = "", params object[]? args) => new() {
        Status = EnumCommandStatus.Success,
        Message = message,
        Args = args
    };

    public static CommandResult Error(string message, params object[]? args) => new() {
        Status = EnumCommandStatus.Error,
        Message = message,
        Args = args
    };
}
