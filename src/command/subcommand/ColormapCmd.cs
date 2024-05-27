using System;
using Vintagestory.API.Common;

namespace livemap.command.subcommand;

public class ColormapCmd : AbstractCommand {
    public ColormapCmd(LiveMap server) : base(server, new[] { "colormap" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        throw new NotImplementedException();
    }
}
