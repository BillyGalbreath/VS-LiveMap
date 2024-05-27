using JetBrains.Annotations;
using livemap.command.subcommand;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command;

[PublicAPI]
public class CommandHandler {
    private readonly LiveMap _server;
    private readonly IChatCommand _chatCommand;

    public CommandHandler(LiveMap server) {
        _server = server;

        _chatCommand = server.Sapi.ChatCommands.Create("livemap")
            .WithRootAlias("map")
            .WithDescription("command.description".ToLang())
            .HandleWith(_ => CommandResult.Success("view-livemap-link").Parse());

        RegisterSubCommand(new ColormapCmd(server));
        RegisterSubCommand(new FullRenderCmd(server));
        RegisterSubCommand(new RadiusRenderCmd(server));
        RegisterSubCommand(new ReloadCmd(server));
        RegisterSubCommand(new StatusCmd(server));
    }

    public void RegisterSubCommand(AbstractCommand command) {
        _chatCommand
            .BeginSubCommands(command.Name)
            .WithDescription(command.Description)
            .RequiresPrivilege(command.Privilege)
            .HandleWith(args => command.Execute(args).Parse())
            .EndSubCommand();
    }

    public void Dispose() {
        _server.Sapi.ChatCommands.UnregisterCommand("livemap");
    }
}
