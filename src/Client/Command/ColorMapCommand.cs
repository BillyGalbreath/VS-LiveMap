using System.Collections.Generic;
using System.Linq;
using LiveMap.Client.Patches;
using LiveMap.Common.Command;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace LiveMap.Client.Command;

public sealed class ColorMapCommand : AbstractClientCommand {
    public ColorMapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override CommandResult Execute(Caller caller, IEnumerable<string> args) {
        caller.SendMessage(CommandResult.Success("command.colormap.started"));

        // we need a world position to sample colors at
        // we'll just use the player's current position
        BlockPos pos = caller.Entity.ServerPos.AsBlockPos;

        // set season override for colors
        GameCalendarPatches.OverridePos = pos;

        // get color of every single known block
        Colormap colormap = new();
        foreach (Block block in caller.Entity.World.Blocks.Where(block => block.Code != null)) {
            // get the base color of this block
            int color = ColorUtil.ReverseColorBytes(block.GetColor(Client.Api, pos));

            // get 30 color samples for this block
            int[] colors = new int[30];
            for (int i = 0; i < 30; i++) {
                // we'll add random seasonal and climate overlay (just like the vanilla minimap)
                colors[i] = ColorUtil.ColorOverlay(color, block.GetRandomColor(Client.Api, pos, BlockFacing.UP, i), 0.6f);
            }

            // store sample colors in the colormap
            colormap.Add(block.Code.ToString()!, colors);
        }

        // remove season override
        GameCalendarPatches.OverridePos = null;

        // send colormap to the server
        Handler.Client.NetworkHandler.SendPacket(new ColormapPacket {
            RawColormap = colormap.Serialize()
        });

        return CommandResult.Success("command.colormap.finished");
    }
}
