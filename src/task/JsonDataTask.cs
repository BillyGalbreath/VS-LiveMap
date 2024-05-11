using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using livemap.layer;
using livemap.render;
using livemap.util;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace livemap.task;

public sealed class JsonDataTask {
    private readonly LiveMapServer _server;

    private Thread? _thread;
    private bool _running;

    private readonly Dictionary<string, long> _markerLastUpdate = new();
    private long _lastSettings;

    public JsonDataTask(LiveMapServer server) {
        _server = server;
    }

    public void Run() {
        if (_running) {
            return;
        }

        _running = true;

        (_thread = new Thread(_ => {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Players();

            string[] markers = Markers(now);

            if (now - _lastSettings > Math.Max(1, 30) * 1000) {
                Settings(markers);
                _lastSettings = now;
            }

            _running = false;
        })).Start();
    }

    private string[] Markers(long now) {
        List<string> list = new();
        foreach ((string? key, Layer? layer) in _server.LayerRegistry) {
            // track this for settings.json
            list.AddIfNotExists(key);

            // check if we should update
            if (now - _markerLastUpdate.GetValueOrDefault(layer.Id, 0) < Math.Max(layer.Interval ?? 0, 0)) {
                continue;
            }

            // update
            Console.Error.WriteLine(_markerLastUpdate.TryAdd(layer.Id, now));
            try {
                string path = Path.Combine(Files.MarkerDir, $"{key}.json");
                string json = JsonConvert.SerializeObject(layer);
                WriteJson(path, json);
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }
        }
        return list.ToArray();
    }

    private void Players() {
        try {
            Dictionary<string, object?> obj = new() { { "players", _server.Api.World.AllOnlinePlayers.Select(Player).ToList() } };
            string path = Path.Combine(Files.JsonDir, "players.json");
            string json = JsonConvert.SerializeObject(obj);
            WriteJson(path, json);
        } catch (Exception e) {
            Console.Error.WriteLine(e.ToString());
        }
    }

    private void Settings(IEnumerable markers) {
        Dictionary<string, object?> obj = new();
        obj.TryAdd("friendlyUrls", true);
        obj.TryAdd("playerList", true);
        obj.TryAdd("playerMarkers", true);
        obj.TryAdd("maxPlayers", _server.Api.Server.Config.MaxClients);
        obj.TryAdd("interval", 30);
        obj.TryAdd("size", _server.Api.WorldManager.Size());
        obj.TryAdd("spawn", _server.Api.World.DefaultSpawnPosition.ToPoint());
        obj.TryAdd("web", new Dictionary<string, object?> { { "tiletype", _server.Config.Web.TileType.Type } });
        obj.TryAdd("zoom", new Dictionary<string, object?> {
            { "def", _server.Config.Zoom.Default },
            { "maxin", _server.Config.Zoom.MaxIn },
            { "maxout", _server.Config.Zoom.MaxOut }
        });
        obj.TryAdd("renderers", Renderers());
        obj.TryAdd("ui", new Dictionary<string, object?> {
            { "attribution", _server.Config.Ui.Attribution },
            { "homepage", _server.Config.Ui.Homepage },
            { "title", _server.Config.Ui.Title },
            { "logo", _server.Config.Ui.Logo },
            { "sidebar", _server.Config.Ui.Sidebar }
        });
        obj.TryAdd("lang", new Dictionary<string, object?> {
            { "pinned", "lang.pinned".ToLang() },
            { "unpinned", "lang.unpinned".ToLang() },
            { "players", "lang.players".ToLang() },
            { "renderers", "lang.renderers".ToLang() },
            { "copy", "lang.copy".ToLang() },
            { "copy-alt", "lang.copy-alt".ToLang() },
            { "paste", "lang.paste".ToLang() },
            { "paste-alt", "lang.paste-alt".ToLang() },
            { "share", "lang.share".ToLang() },
            { "share-alt", "lang.share-alt".ToLang() },
            { "center", "lang.center".ToLang() },
            { "center-alt", "lang.center-alt".ToLang() },
            { "notif-copy", "lang.notif-copy".ToLang() },
            { "notif-copy-failed", "lang.notif-copy-failed".ToLang() },
            { "notif-paste", "lang.notif-paste".ToLang() },
            { "notif-paste-failed", "lang.notif-paste-failed".ToLang() },
            { "notif-paste-invalid", "lang.notif-paste-invalid".ToLang() },
            { "notif-share", "lang.notif-share".ToLang() },
            { "notif-share-failed", "lang.notif-share-failed".ToLang() },
            { "notif-center", "lang.notif-center".ToLang() }
        });
        obj.TryAdd("markers", markers);
        try {
            string path = Path.Combine(Files.JsonDir, "settings.json");
            string json = JsonConvert.SerializeObject(obj);
            WriteJson(path, json);
        } catch (Exception e) {
            Console.Error.WriteLine(e.ToString());
        }
    }

    private Dictionary<string, object?>[] Renderers() {
        List<Dictionary<string, object?>> list = new();
        Dictionary<string, Renderer.Builder> renderers = new(_server.RendererRegistry);
        foreach ((string? key, _) in renderers) {
            Dictionary<string, object?> obj = new();
            obj.TryAdd("id", key);
            obj.TryAdd("icon", "" /*renderer.Icon*/); // todo
            list.AddIfNotExists(obj);
        }
        return list.ToArray();
    }

    private static Dictionary<string, object?> Player(IPlayer player) {
        Dictionary<string, object?> obj = new();
        obj.TryAdd("id", player.PlayerUID);
        obj.TryAdd("name", player.PlayerName);
        obj.TryAdd("avatar", player.Entity.GetAvatar());
        obj.TryAdd("role", player.Role.Code); // todo add role color to name
        obj.TryAdd("pos", player.GetPoint());
        obj.TryAdd("yaw", 90 - ((player.Entity?.SidedPos?.Yaw ?? 0) * (180.0 / Math.PI)));
        obj.TryAdd("health", player.GetHealth());
        obj.TryAdd("satiety", player.GetSatiety());
        return obj;
    }

    private static void WriteJson(string path, string json) {
        FileInfo file = new(path);
        GamePaths.EnsurePathExists(file.Directory!.FullName);
        File.WriteAllText(file.FullName, json);
    }

    public void Dispose() {
        _thread?.Interrupt();
    }
}
