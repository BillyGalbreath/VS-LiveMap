using LiveMap.Common.Command;

namespace LiveMap.Client.Command;

public sealed class ClientCommandHandler : CommandHandler {
    public LiveMapClient Client { get; }

    public ClientCommandHandler(LiveMapClient client) : base(client) {
        Client = client;
    }

    protected override void RegisterSubCommands() {
        _ = new ColorMapCommand(this);
    }
}
