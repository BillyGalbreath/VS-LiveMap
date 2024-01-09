using System.Collections.Generic;
using System.Linq;
using LiveMap.Common.Command;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace LiveMap.Client.Command;

public class ColorMapCommand : AbstractClientCommand {
    private static bool _running;

    public ColorMapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        if (_running) {
            return CommandResult.Success("command.colormap.running");
        }

        _running = true;

        IWorldAccessor world = caller.Entity.World;
        IGameCalendar calendar = world.Calendar;

        Handler.Client.Api.ShowChatMessage(Lang.Success("command.colormap.started"));

        // we need a world position to sample colors at
        // we'll just use the player's current position
        BlockPos pos = caller.Entity.Pos.AsBlockPos;

        // populate block colors
        Colormap colormap = new();
        IList<Block> blocks = world.Blocks;

        // set season override for colors
        float? seasonOverride = calendar.SeasonOverride;
        calendar.SetSeasonOverride(0.5F);

        foreach (Block block in blocks.Where(block => block.Code != null)) {
            int color = ColorUtil.ReverseColorBytes(block.GetColor(Handler.Client.Api, pos));

            int[] colors = new int[30];
            for (int i = 0; i < 30; i++) {
                colors[i] = ColorUtil.ColorOverlay(color,
                    block.GetRandomColor(Handler.Client.Api, pos, BlockFacing.UP, i), 0.6f);
            }

            colormap.Add(block.Code.ToString()!, colors);
        }

        // remove season override
        calendar.SetSeasonOverride(seasonOverride);

        // send back to server
        Handler.Client.NetworkHandler.SendPacket(new ColormapPacket {
            RawColormap = colormap.Serialize()
        });

        _running = false;

        return CommandResult.Success("command.colormap.finished");
    }
}
