using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace LiveMap.Common.Command;

public abstract class CommandHandler {
    private readonly Dictionary<string, AbstractCommand> commands = new();

    public LiveMap Mod { get; }

    protected CommandHandler(LiveMap mod) {
        Mod = mod;

        Mod.Api.ChatCommands
            .Create("livemap")
            .WithDescription(Lang.Get("command-description"))
            .RequiresPrivilege("livemap.admin")
            .WithArgs(new WordArgParser("command", false, commands.Keys.ToArray()))
            .HandleWith(Execute);


        // ReSharper disable once VirtualMemberCallInConstructor
        // we're not using any uninitialized instanced fields in this
        // method so this odd language behavior doesn't concern us
        RegisterSubCommands();
    }

    protected abstract void RegisterSubCommands();

    public void RegisterSubCommand(string name, AbstractCommand command) {
        commands.Add(name, command);
    }

    private TextCommandResult Execute(TextCommandCallingArgs args) {
        if (args.Parsers[0].IsMissing) {
            return TextCommandResult.Success(Lang.Error("unknown-sub-command"));
        }

        string name = args[0].ToString()!.Trim().ToLower();
        AbstractCommand? command = commands!.Get(name);
        if (command == null) {
            return TextCommandResult.Success(Lang.Error("unknown-sub-command"));
        }

        List<string> argsList = new();
        for (int i = 1; i < args.ArgCount; i++) {
            argsList.Add(args[i].ToString()!);
        }

        CommandResult result = command.Execute(args.Caller, argsList);

        return TextCommandResult.Success(result switch {
            { Status: EnumCommandStatus.Error } => Lang.Error(result.Message, result.Args),
            { Message.Length: > 0 } => Lang.Success(result.Message, result.Args),
            _ => ""
        });
    }

    public void Dispose() {
        commands.Clear();
    }
}
