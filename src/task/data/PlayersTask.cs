using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.util;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace livemap.task.data;

public class PlayersTask : JsonTask {
    public PlayersTask(LiveMap server) : base(server) { }

    protected override async Task TickAsync(CancellationToken cancellationToken) {
        List<Dictionary<string, object?>> players = new();

        List<IPlayer> onlinePlayers = new(_server.Sapi.World.AllOnlinePlayers);
        foreach (IPlayer player in onlinePlayers) {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            if (player.Entity == null) {
                return;
            }

            Dictionary<string, object?> dict = new();
            dict.TryAdd("id", player.PlayerUID);
            dict.TryAdd("name", player.PlayerName);
            dict.TryAdd("avatar", player.Entity.GetAvatar());
            dict.TryAdd("role", player.Role.Code); // todo add role color to name
            dict.TryAdd("pos", player.GetPoint());
            dict.TryAdd("yaw", 90 - ((player.Entity.SidedPos?.Yaw ?? 0) * (180.0 / Math.PI)));
            dict.TryAdd("health", player.GetHealth());
            dict.TryAdd("satiety", player.GetSatiety());
            players.Add(dict);
        }

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        string json = JsonConvert.SerializeObject(new Dictionary<string, object?> {
            {
                "players", players
            }
        });

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        await Files.WriteJsonAsync(Path.Combine(Files.JsonDir, "players.json"), json, cancellationToken);
    }
}
