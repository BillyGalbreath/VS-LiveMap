using livemap.common.command;

namespace livemap.client.command;

public sealed class ClientCommandHandler : CommandHandler {
    public LiveMapClient Client { get; }

    public ClientCommandHandler(LiveMapClient client) : base(client) {
        Client = client;
    }

    protected override void RegisterSubCommands() {
        _ = new ColorMapCommand(this);
    }
}
