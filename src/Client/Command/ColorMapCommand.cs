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

    public override CommandResult Execute(Caller caller, IEnumerable<string> _) {
        caller.SendMessage(CommandResult.Success("command.colormap.started"));

        // colormap to store colors
        Colormap colormap = new();

        // we need a world position to sample colors at
        // we'll just use the player's current position
        BlockPos pos = caller.Entity.ServerPos.AsBlockPos;

        // set season override for colors
        GameCalendarPatches.OverridePos = pos;

        // get color of every single known block
        foreach (Block block in caller.Entity.World.Blocks.Where(block => block.Code != null)) {
            ProcessBlock(block, pos, colormap);
        }

        // remove season override
        GameCalendarPatches.OverridePos = null;

        // send colormap to the server
        Handler.Client.NetworkHandler.SendPacket(new ColormapPacket {
            RawColormap = colormap.Serialize()
        });

        return CommandResult.Success("command.colormap.finished");
    }

    private void ProcessBlock(Block block, BlockPos pos, Colormap colormap) {
        // get the base color of this block - game stores these in reverse byte order for some reason
        int argb = Color.Reverse(block.GetColor(Client.Api, pos));

        // get 30 color samples for this block
        uint[] colors = new uint[30];
        for (int i = 0; i < 30; i++) {
            // blend the base color with a random color
            colors[i] = (uint)Color.Blend(argb, block.GetRandomColor(Client.Api, pos, BlockFacing.UP, i));
        }

        // store sample colors in the colormap
        colormap.Add(block.Code.ToString()!, colors);
    }
}
