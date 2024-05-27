using System;
using System.Linq;
using livemap.data;
using livemap.network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace livemap;

public sealed class LiveMapClient {
    private static BlockPos? _overridePos;

    private readonly ICoreClientAPI _api;
    private readonly ILogger _logger;

    private IClientNetworkChannel? _channel;

    public LiveMapClient(LiveMapMod mod, ICoreClientAPI api) {
        _api = api;
        _logger = mod.Mod.Logger;

        _channel = api.Network.RegisterChannel(mod.Mod.Info.ModID)
            .RegisterMessageType<ColormapPacket>()
            .SetMessageHandler<ColormapPacket>(_ => {
                _logger.Event("Received colormap request from server");

                if (!api.World.Player.HasPrivilege(Privilege.root)) {
                    _logger.Event("No privilege to use this mod");
                    return;
                }

                Colormap? colormap = GenerateColormap();

                if (colormap != null && _channel is { Connected: true }) {
                    _logger.Event("Sending generated colormap to server");
                    _channel.SendPacket(new ColormapPacket { RawColormap = colormap.Serialize() });
                }
            });
    }

    private Colormap? GenerateColormap() {
        if (_overridePos != null) {
            return null;
        }

        Colormap colormap = new();
        EntityPlayer player = _api.World.Player.Entity;
        _overridePos = player.SidedPos.AsBlockPos;

        try {
            foreach (Block block in player.World.Blocks.Where(block => block.Code != null)) {
                if (_overridePos == null) {
                    return null;
                }
                uint baseColor = Color.Reverse((uint)block.GetColor(_api, _overridePos));
                uint[] colors = new uint[30];
                for (int i = 0; i < colors.Length; i++) {
                    uint randColor = (uint)block.GetRandomColor(_api, _overridePos, BlockFacing.UP, i);
                    uint color = Color.Blend(baseColor, randColor, 0.4F);
                    colors[i] = color & 0xFFFFFF;
                }
                colormap.Add(block.Code.ToString(), colors);
            }
        } catch (Exception e) {
            _logger.Error(e.ToString());
        }

        _overridePos = null;

        return colormap;
    }

    public void Dispose() {
        _channel = null;
        _overridePos = null;
    }
}
