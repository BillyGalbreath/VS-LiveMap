using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.configuration;
using livemap.util;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace livemap.task.data;

public class PlayersTask : JsonTask {
    public PlayersTask(LiveMap server) : base(server) { }

    protected override async Task TickAsync(CancellationToken cancellationToken) {
        List<Dictionary<string, object?>> players = new();

        List<IPlayer> onlinePlayers = _server.Config.Layers.Players.Enabled ? new List<IPlayer>(_server.Sapi.World.AllOnlinePlayers) : new List<IPlayer>();
        foreach (IPlayer player in onlinePlayers) {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            EntityPlayer entity = player.Entity;
            if (entity == null) {
                return;
            }

            Players config = _server.Config.Layers.Players;
            if (config.HideSpectators && player.WorldData.CurrentGameMode == EnumGameMode.Spectator) {
                return;
            }

            if (config.HideIfSneaking && entity.Controls.Sneak) {
                return;
            }

            if (config.HideUnderBlocks && entity.SidedPos.Y < entity.World.BlockAccessor.GetRainMapHeightAt(entity.SidedPos.AsBlockPos)) {
                return;
            }

            Dictionary<string, object?> dict = new();
            dict.TryAdd("id", player.PlayerUID);
            dict.TryAdd("name", player.PlayerName);
            dict.TryAdd("avatar", entity.GetAvatar());
            dict.TryAdd("role", player.Role.Code); // todo add role color to name
            dict.TryAdd("pos", player.GetPoint());
            dict.TryAdd("yaw", 90 - ((entity.SidedPos?.Yaw ?? 0) * (180.0 / Math.PI)));
            dict.TryAdd("health", player.GetHealth());
            dict.TryAdd("satiety", player.GetSatiety());
            players.Add(dict);
        }

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        string json = JsonConvert.SerializeObject(new Dictionary<string, object?> {
            { "interval", _server.Config.Layers.Players.UpdateInterval },
            { "hidden", !_server.Config.Layers.Players.DefaultShowLayer },
            { "players", players }
        });

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        await Files.WriteJsonAsync(Path.Combine(Files.JsonDir, "players.json"), json, cancellationToken);
    }
}
