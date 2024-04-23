using Vintagestory.API.Common;

namespace livemap.common.command;

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
