using LiveMap.Common.Command;

namespace LiveMap.Server.Command;

public sealed class ServerCommandHandler : CommandHandler {
    public LiveMapServer Server { get; }

    public ServerCommandHandler(LiveMapServer server) : base(server) {
        Server = server;
    }

    protected override void RegisterSubCommands() {
        _ = new FullRenderCommand(this);
    }
}
