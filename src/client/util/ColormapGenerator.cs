using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using HarmonyLib;
using livemap.common;
using livemap.common.network.packet;
using livemap.common.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

namespace livemap.client.util;

[HarmonyPatch]
public class ColormapGenerator {
    private static BlockPos? _overridePos;

    private readonly LiveMapClient _client;

    private Thread? _thread;
    private bool _running;

    private int _total;
    private int _count;

    public ColormapGenerator(LiveMapClient client) {
        _client = client;
    }

    public bool Running() {
        return _running;
    }

    public float Progress() {
        return (float)_count / _total;
    }

    public void Cancel() {
        _count = 0;
        _total = 0;
        _running = false;
        _thread?.Interrupt();
        _thread = null;
        _overridePos = null;
    }

    public void Run() {
        if (_running) {
            return;
        }

        // reset everything, just in case
        Cancel();

        // handle this on its own thread
        (_thread = new Thread(AsyncRun)).Start();
    }

    private void AsyncRun() {
        // lets go! \o/
        _running = true;

        // who are we and what do we want?!
        IPlayer player = _client.Api.World.Player;
        EntityPlayer entity = player.Entity;
        IList<Block> blocks = entity.World.Blocks;

        // get our counts ready to progress bar
        _total = blocks.Count;
        _count = 0;

        // colormap to store colors
        Colormap colormap = new();

        // we need a world position to sample colors at
        // we'll just use the player's current position
        BlockPos pos = entity.SidedPos.AsBlockPos;

        // set season override for colors
        _overridePos = pos;

        // get color of every single known block
        foreach (Block block in blocks.Where(block => block.Code != null)) {
            if (!_running) {
                // short circuit for stop/cancel
                return;
            }

            // process one block at a time
            ProcessBlock(block, pos, colormap);

            // todo - remove this in production
            // slow the process down to test the gui progress bar
            if (_count % 10 == 0) {
                try {
                    Thread.Sleep(1);
                } catch (Exception) {
                    // ignore
                }
            }
        }

        // remove season override
        _overridePos = null;

        // send colormap to the server
        _client.NetworkHandler.SendPacket(new ColormapPacket { RawColormap = colormap.Serialize() });

        // whew!
        _running = false;
    }

    private void ProcessBlock(Block block, BlockPos pos, Colormap colormap) {
        // get the base color of this block - game stores these in reverse byte order for some reason
        int argb = Color.Reverse(block.GetColor(_client.Api, pos));

        // get 30 color samples for this block
        uint[] colors = new uint[30];
        for (int i = 0; i < 30; i++) {
            // blend the base color with a random color
            colors[i] = (uint)Color.Blend(argb, block.GetRandomColor(_client.Api, pos, BlockFacing.UP, i));
        }

        // store sample colors in the colormap
        colormap.Add(block.Code.ToString()!, colors);

        // finished this block
        _count++;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCalendar), "get_YearRel")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static bool PreYearRel(IGameCalendar __instance, ref float __result) {
        if (_overridePos == null) {
            return true;
        }

        __result = __instance.GetHemisphere(_overridePos) == EnumHemisphere.North ? 0.6f : 0.1f;
        return false;
    }
}
