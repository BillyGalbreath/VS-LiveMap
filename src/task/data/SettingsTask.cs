using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.render;
using livemap.util;
using Newtonsoft.Json;

namespace livemap.task.data;

public sealed class SettingsTask : JsonTask {
    public SettingsTask(LiveMapServer server) : base(server) { }

    protected override async Task TickAsync(CancellationToken cancellationToken) {
        Dictionary<string, object?> dict = new();
        dict.TryAdd("friendlyUrls", true);
        dict.TryAdd("playerList", true);
        dict.TryAdd("playerMarkers", true);
        dict.TryAdd("maxPlayers", _server.Api.Server.Config.MaxClients);
        dict.TryAdd("interval", 30);
        dict.TryAdd("size", _server.Api.WorldManager.Size());
        dict.TryAdd("spawn", _server.Api.World.DefaultSpawnPosition.ToPoint());
        dict.TryAdd("web", new Dictionary<string, object?> {
            { "tiletype", _server.Config.Web.TileType.Type }
        });
        dict.TryAdd("zoom", new Dictionary<string, object?> {
            { "def", _server.Config.Zoom.Default },
            { "maxin", _server.Config.Zoom.MaxIn },
            { "maxout", _server.Config.Zoom.MaxOut }
        });
        dict.TryAdd("renderers", Renderers(cancellationToken));
        dict.TryAdd("ui", new Dictionary<string, object?> {
            { "attribution", _server.Config.Ui.Attribution },
            { "homepage", _server.Config.Ui.Homepage },
            { "title", _server.Config.Ui.Title },
            { "logo", _server.Config.Ui.Logo },
            { "sidebar", _server.Config.Ui.Sidebar }
        });
        dict.TryAdd("lang", new Dictionary<string, object?> {
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

        try {
            string json = JsonConvert.SerializeObject(dict);

            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            await Files.WriteJsonAsync(Path.Combine(Files.JsonDir, "settings.json"), json, cancellationToken);
        } catch (Exception e) {
            await Console.Error.WriteLineAsync(e.ToString());
        }
    }

    private Dictionary<string, string>[] Renderers(CancellationToken cancellationToken) {
        List<Dictionary<string, string>> dict = new();
        List<Renderer> renderers = new(_server.RendererRegistry.Values);
        foreach (Renderer renderer in renderers) {
            if (cancellationToken.IsCancellationRequested) {
                break;
            }

            Dictionary<string, string> obj = new();
            obj.TryAdd("id", renderer.Id);
            obj.TryAdd("icon", "" /*renderer.Icon*/); // todo
            dict.AddIfNotExists(obj);
        }
        return dict.ToArray();
    }
}
