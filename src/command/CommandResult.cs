using JetBrains.Annotations;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command;

[PublicAPI]
public class CommandResult {
    public required EnumCommandStatus Status;
    public required string Message;
    public object[]? Args;

    public static CommandResult Success(string message = "", params object[]? args) => new() {
        Status = EnumCommandStatus.Success,
        Message = $"command.{message}",
        Args = args
    };

    public static CommandResult Error(string message, params object[]? args) => new() {
        Status = EnumCommandStatus.Error,
        Message = $"command.{message}",
        Args = args
    };

    public TextCommandResult Parse() {
        return TextCommandResult.Success(Format().ToLang(Message.ToLang(Args)));
    }

    private string Format() {
        return this switch {
            { Status: EnumCommandStatus.Error } => "command.error",
            { Message.Length: > 0 } => "command.success",
            _ => ""
        };
    }
}
