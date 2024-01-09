using LiveMap.Common.Command;
using LiveMap.Common.Util;

namespace LiveMap.Server.Command;

public sealed class ServerCommandHandler : CommandHandler {
    private LiveMapServer Server { get; }

    public ServerCommandHandler(LiveMapServer server) : base(server) {
        Server = server;

        Server.Api.Permissions.RegisterPrivilege("livemap.admin", Lang.Get("command.livemap.description"), false);
    }

    protected override void RegisterSubCommands() {
        _ = new FullRenderCommand(this);
    }
}
