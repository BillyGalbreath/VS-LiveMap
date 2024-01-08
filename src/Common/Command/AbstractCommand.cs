using System.Collections.Generic;
using Vintagestory.API.Common;

namespace LiveMap.Common.Command;

public abstract class AbstractCommand {
    protected readonly CommandHandler Handler;

    protected AbstractCommand(CommandHandler handler) {
        Handler = handler;
    }

    public abstract CommandResult Execute(Caller caller, List<string> args);

    protected static CommandResult Success(string message = "", params object[]? args) => new() {
        Status = EnumCommandStatus.Success,
        Message = message,
        Args = args
    };

    protected static CommandResult Error(string message, params object[]? args) => new() {
        Status = EnumCommandStatus.Error,
        Message = message,
        Args = args
    };
}

public class CommandResult {
    public required EnumCommandStatus Status;
    public required string Message;
    public object[]? Args;
}
