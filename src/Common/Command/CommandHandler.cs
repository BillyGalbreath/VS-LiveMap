using System.Collections.Generic;
using System.Linq;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace LiveMap.Common.Command;

public abstract class CommandHandler {
    private readonly Dictionary<string, AbstractCommand> _commands = new();

    protected CommandHandler(LiveMap mod) {
        mod.Api.ChatCommands
            .Create("livemap")
            .WithDescription(Lang.Get("command.livemap.description"))
            .RequiresPrivilege("livemap.admin")
            .WithArgs(new WordArgParser("command", false, _commands.Keys.ToArray()))
            .HandleWith(VanillaExecute);

        // ReSharper disable once VirtualMemberCallInConstructor
        // we're not using any uninitialized instanced fields in this
        // method so this odd language behavior doesn't concern us
        RegisterSubCommands();
    }

    protected abstract void RegisterSubCommands();

    public void RegisterSubCommand(string name, AbstractCommand command) {
        _commands.Add(name, command);
    }

    protected virtual TextCommandResult VanillaExecute(TextCommandCallingArgs args) {
        CommandResult result = InternalExecute(args);

        return TextCommandResult.Success(result switch {
            { Status: EnumCommandStatus.Error } => Lang.Error(result.Message, result.Args),
            { Message.Length: > 0 } => Lang.Success(result.Message, result.Args),
            _ => ""
        });
    }

    protected CommandResult InternalExecute(TextCommandCallingArgs args) {
        if (args.Parsers[0].IsMissing) {
            return CommandResult.Error("command.unknown-subcommand");
        }

        string name = args[0].ToString()!.Trim().ToLower();
        AbstractCommand? command = _commands!.Get(name);
        if (command == null) {
            return CommandResult.Error("command.unknown-subcommand");
        }

        List<string> argsList = new();
        for (int i = 1; i < args.ArgCount; i++) {
            argsList.Add(args[i].ToString()!);
        }

        return command.Execute(args.Caller, argsList);
    }

    public void Dispose() {
        _commands.Clear();
    }
}
