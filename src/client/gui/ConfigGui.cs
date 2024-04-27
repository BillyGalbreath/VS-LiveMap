using System;
using System.Collections.Generic;
using ConfigLib;
using livemap.client.gui.settings;
using livemap.common.extensions;
using livemap.common.network;
using livemap.common.util;
using Vintagestory.API.Server;

namespace livemap.client.gui;

public class ConfigGui : Gui {
    private readonly LiveMapClient _client;

    private readonly List<Gui> _guis = new();

    private bool _alreadyRequestedConfig;

    public ConfigGui(LiveMapClient client) {
        _client = client;

        ConfigLibModSystem? configlib = _client.Api.ModLoader.GetModSystem<ConfigLibModSystem>();
        configlib.ConfigWindowClosed += OnClose;
        configlib.RegisterCustomConfig(LiveMapMod.Id, (_, controlButtons) => {
            try {
                return Draw(controlButtons);
            } catch (Exception e) {
                Logger.Error(e.ToString());
                return new ControlButtons(false);
            }
        });

        _guis.Add(new ColormapSettings(client));
        _guis.Add(new HttpdSettings(client));
        _guis.Add(new ZoomSettings(client));
    }

    private ControlButtons Draw(ControlButtons controlButtons) {
        if (!_client.Api.World.Player.HasPrivilege(Privilege.root)) {
            Text($"\n{"access-denied".ToLang()}", true, 0xFFFF4040);
            return new ControlButtons(false);
        }

        if (_client.Config == null) {
            Text($"{"no-data-to-display".ToLang()}", true, 0xFFFF4040);
            // ReSharper disable once InvertIf
            if (!_alreadyRequestedConfig) {
                _client.NetworkHandler.SendPacket(new ConfigPacket());
                _alreadyRequestedConfig = true;
            }
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

    public override void Draw() {
        _guis.ForEach(gui => gui.Draw());
    }

    public override void OnClose() {
        _guis.ForEach(gui => gui.OnClose());
    }

    public void Dispose() {
        _guis.Clear();
    }
}
