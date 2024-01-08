using System.Collections.Generic;
using System.Linq;
using LiveMap.Common;
using LiveMap.Common.Command;
using LiveMap.Common.Network;
using LiveMap.Common.Util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace LiveMap.Client.Command;

public class ColorMapCommand : AbstractClientCommand {
    private static bool _running;

    public ColorMapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override CommandResult Execute(Caller caller, List<string> args) {
        if (_running) {
            return Success("colormap-running");
        }

        _running = true;

        ICoreClientAPI capi = ClientHandler.Client.Api;
        IClientWorldAccessor world = capi.World;
        IClientGameCalendar calendar = world.Calendar;

        Send(Lang.Success("colormap-started"));

        // set season override for colors
        // todo - allow command to set month (or not?)
        float? seasonOverride = calendar.SeasonOverride;
        calendar.SetSeasonOverride(0.5F);

        // we need a world position to sample colors at
        // we'll just use the player's current position
        BlockPos pos = world.Player.Entity.Pos.AsBlockPos;

        // populate block colors
        BlockColors blockColors = new();
        foreach (Block block in world.Blocks.Where(block => block.Code != null)) {
            int color = ColorUtil.ReverseColorBytes(block.GetColor(capi, pos));

            int[] colors = new int[30];
            for (int i = 0; i < 30; i++) {
                colors[i] = ColorUtil.ColorOverlay(color, block.GetRandomColor(capi, pos, BlockFacing.UP, i), 0.6f);
            }

            blockColors.Colors.Add(block.Code.ToString()!, colors);
        }

        // remove season override
        calendar.SetSeasonOverride(seasonOverride);

        // send back to server
        Handler.Mod.NetworkHandler.SendPacket(new BlockColorsPacket {
            RawDataColors = blockColors.Serialize()
        });

        _running = false;

        return Success("colormap-finished");
    }
}
