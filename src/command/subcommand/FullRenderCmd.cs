using System.Threading;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class FullRenderCmd : AbstractCommand {
    public FullRenderCmd(LiveMap server) : base(
        server,
        new[] { "fullrender" }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        if (_server.Colormap.Count == 0) {
            return "fullrender.missing-colormap".CommandError();
        }

        new Thread(_ => {
            if (_server.RenderTaskManager != null) {
                _server.RenderTaskManager.QueueAll();
                _server.Sapi.AutoSaveNow();
            }
        }).Start();

        return "fullrender.started".CommandSuccess();
    }
}
