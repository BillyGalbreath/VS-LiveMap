using LiveMap.Common.Command;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Lang = LiveMap.Common.Util.Lang;

namespace LiveMap.Server.Command;

public sealed class ServerCommandHandler : CommandHandler {
    internal LiveMapServer Server { get; }

    public ServerCommandHandler(LiveMapServer server) : base(server) {
        Server = server;

        Server.Api.Permissions.RegisterPrivilege("livemap.admin", Lang.Get("command.livemap.description"), false);
    }

    protected override void RegisterSubCommands() {
        _ = new ColormapCommand(this);
        _ = new FullRenderCommand(this);
        _ = new ReloadCommand(this);
    }

    protected override TextCommandResult VanillaExecute(TextCommandCallingArgs args) {
        CommandResult result = InternalExecute(args);

        if (result.Message.Length == 0) {
            return TextCommandResult.Deferred;
        }

        bool error = result.Status == EnumCommandStatus.Error;

        if (args.Caller.Player is IServerPlayer player) {
            string message = error ? Lang.Error(result.Message, result.Args) : Lang.Success(result.Message, result.Args);
            player.SendMessage(GlobalConstants.GeneralChatGroup, message, error ? EnumChatType.CommandError : EnumChatType.CommandSuccess);
        } else {
            Logger.Info($"{(error ? "&c" : "&a")}{Lang.Get(result.Message, result.Args)}&r");
        }

        return TextCommandResult.Deferred;
    }
}
