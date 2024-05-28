using System;
using System.Threading;
using livemap.command.argument;
using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace livemap.command.subcommand;

public class ApothemRenderCmd : AbstractCommand {
    public ApothemRenderCmd(LiveMap server) : base(
        server,
        new[] { "apothemrender", "radiusrender", "rangerender" },
        argParsers: new ICommandArgumentParser[] {
            new ApothemArgParser("apothem"),
            new CenterPositionArgParser("center", server.Sapi)
        }
    ) { }

    public override TextCommandResult Execute(TextCommandCallingArgs args) {
        IWorldManagerAPI world = _server.Sapi.WorldManager;
        int mapX = world.MapSizeX;
        int mapZ = world.MapSizeZ;

        int apothem = (int)args[0];
        if (apothem < 0 || apothem > Math.Max(mapX, mapZ)) {
            return "apothemrender.out-of-bounds".CommandError();
        }

        Vec2i? pos = args[1] as Vec2i;
        if (pos == null) {
            if (args.Caller.Player == null) {
                return "apothemrender.missing-center-or-player".CommandError();
            }
            EntityPos sided = args.Caller.Player.Entity.SidedPos;
            pos = new Vec2i((int)sided.X, (int)sided.Z);
        }

        new Thread(_ => {
            _server.RenderTaskManager.QueueAll(
                new Vec2i(Math.Max(0, pos.X - apothem) >> 5, Math.Max(0, pos.Y - apothem) >> 5),
                new Vec2i(Math.Min(mapX, pos.X + apothem) >> 5, Math.Min(mapZ, pos.Y + apothem) >> 5)
            );
            _server.Sapi.AutoSaveNow();
        }).Start();

        return "apothemrender.started".CommandSuccess();
    }
}
