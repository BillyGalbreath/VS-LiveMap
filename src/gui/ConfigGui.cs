using System;
using System.Collections.Generic;
using ConfigLib;
using livemap.gui.settings;
using livemap.util;
using Vintagestory.API.Server;

namespace livemap.gui;

public class ConfigGui : Gui {
    private readonly List<Gui> _guis = new();

    public ConfigGui(LiveMapClient client) : base(client) {
        ConfigLibModSystem? configlib = _client.Api.ModLoader.GetModSystem<ConfigLibModSystem>();
        configlib.ConfigWindowOpened += OnOpen;
        configlib.ConfigWindowClosed += OnClose;
        configlib.RegisterCustomConfig(LiveMapMod.Id, (_, controlButtons) => {
            try {
                return Draw(controlButtons);
            } catch (Exception) {
                return new ControlButtons(false);
            }
        });

        _guis.Add(new ColormapSettings(client));
        _guis.Add(new HttpdSettings(client));
        _guis.Add(new WebSettings(client));
        _guis.Add(new ZoomSettings(client));
    }

    public override void OnOpen() {
        if (_client.Api.World.Player.HasPrivilege(Privilege.root)) {
            _client.RequestConfig();
        }
        _guis.ForEach(gui => gui.OnOpen());
    }

    public override void OnClose() {
        _guis.ForEach(gui => gui.OnClose());
    }

    public override void Draw() {
        _guis.ForEach(gui => gui.Draw());
    }

    public void Dispose() {
        _guis.Clear();
    }

    private ControlButtons Draw(ControlButtons controlButtons) {
        if (!_client.Api.World.Player.HasPrivilege(Privilege.root)) {
            Text($"{"access-denied".ToLang()}", true, 0xFFFF4040);
            return new ControlButtons(false);
        }

        if (_client.Config == null) {
            Text($"{"no-data-to-display".ToLang()}", true, 0xFFFF4040);
            _client.RequestConfig();
            return new ControlButtons(false);
        }

        if (controlButtons.Save) {
            // saves changes to config
            // todo send values back to server
            Console.WriteLine("SAVE");
        }

        if (controlButtons.Restore) {
            // retrieves values from config
            // todo request new values from server
            Console.WriteLine("RESTORE");
        }

        if (controlButtons.Reload) {
            // applies settings changes
            // todo not needed
            Console.WriteLine("RELOAD");
        }

        if (controlButtons.Defaults) {
            // sets settings to default values
            // todo do we want original defaults or current defaults?
            Console.WriteLine("DEFAULTS");
        }

        Draw();

        return new ControlButtons(true) { Reload = false };
    }
}
