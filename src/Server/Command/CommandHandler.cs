using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace LiveMap.Server.Command;

public class CommandHandler {
    private readonly LiveMapServer server;
    private readonly Dictionary<string, AbstractCommand> commands = new();

    public LiveMapServer Server { get { return server; } }

    public CommandHandler(LiveMapServer server) {
        this.server = server;

        RegisterSubCommands();

        IChatCommand cmd = server.API.ChatCommands
            .Create("livemap")
            .WithDescription(Lang.Get("command-description"))
            .RequiresPrivilege("livemap.admin")
            .WithArgs(new WordArgParser("command", false, commands.Keys.ToArray()))
            .HandleWith(Execute);
    }

    private void RegisterSubCommands() {
        _ = new ColorMapCommand(this);
    }

    public void RegisterSubCommand(string name, AbstractCommand command) {
        commands.Add(name, command);
    }

    private TextCommandResult Execute(TextCommandCallingArgs args) {
        if (args.Parsers[0].IsMissing) {
            return TextCommandResult.Success(Lang.Get("version-info"));
        }

        string name = args[0].ToString().Trim().ToLower();
        AbstractCommand command = commands.Get(name);
        if (command == null) {
            return TextCommandResult.Success("unknown-command");
        }

        List<string> argsList = new();
        for (int i = 1; i < args.ArgCount; i++) {
            argsList.Add(args[i].ToString());
        }

        return command.Execute(args.Caller, argsList);
    }

    public void Dispose() {
        commands.Clear();
    }
}
