using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.Common.Database;

namespace livemap.command.subcommand;

public class FullRenderCmd(LiveMap server) : AbstractCommand(server, ["fullrender"]) {
    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        if (_server.Colormap.Count == 0) {
            return "fullrender.missing-colormap".CommandError();
        }

        new Thread(_ => {
            if (_server.RenderTaskManager == null) {
                return;
            }

            // queue up all existing chunks in the whole world
            foreach (ChunkPos regionPos in _server.RenderTaskManager.ChunkLoader.GetAllMapRegionPositions()) {
                _server.RenderTaskManager.Queue(regionPos.X, regionPos.Z);
            }

            // trigger autosave to process queue
            _server.Sapi.AutoSaveNow();
        }).Start();

        return "fullrender.started".CommandSuccess();
    }
}
