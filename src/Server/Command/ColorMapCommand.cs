using LiveMap.Common.Network;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace LiveMap.Server.Command;

public class ColorMapCommand : AbstractCommand {
    public ColorMapCommand(CommandHandler handler) : base(handler) {
        handler.RegisterSubCommand("colormap", this);
    }

    public override TextCommandResult Execute(Caller caller, List<string> args) {
        if (caller.Player is not IServerPlayer player) {
            // we need a connected player to get block colors from their client
            return TextCommandResult.Success(Lang.Get("player-only-command"));
        }

        // ask player for their client's block colors
        handler.Server.NetworkHandler.SendPacket(new CanIHazColorsPacket(), player);

        return TextCommandResult.Success("colormap-asked-client");
    }
}
