using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ReloadCmd(LiveMap server) : AbstractCommand(server, ["reload"]) {
    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        _server.Reload();

        return "reload.done".CommandSuccess();
    }
}
