using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class StatusCmd : AbstractCommand {
    public StatusCmd(LiveMap server) : base(
        server,
        new[] { "status", "progress" }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        return Server.RenderTaskManager.Status();
    }
}
