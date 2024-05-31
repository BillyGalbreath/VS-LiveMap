using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ReloadCmd : AbstractCommand {
    public ReloadCmd(LiveMap server) : base(
        server,
        new[] { "reload" }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        _server.Reload();

        return "reload.done".CommandSuccess();
    }
}
