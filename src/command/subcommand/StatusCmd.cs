using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class StatusCmd : AbstractCommand {
    public StatusCmd(LiveMap server) : base(
        server,
        new[] { "status", "progress" }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        (int buffer, int process) = _server.RenderTaskManager?.GetCounts() ?? (0, 0);
        return process == 0 ? "status.idle".CommandSuccess(buffer) : "status.running".CommandSuccess(process);
    }
}
