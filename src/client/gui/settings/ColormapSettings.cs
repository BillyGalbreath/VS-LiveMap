using System.Numerics;
using ImGuiNET;
using livemap.client.gui.modal;
using livemap.common.util;
using VSImGui;

namespace livemap.client.gui.settings;

public class ColormapSettings : Gui {
    private readonly ColormapModal _modal;

    public ColormapSettings(LiveMapClient client) {
        _modal = new ColormapModal(client);
    }

    public override void OnClose() {
        _modal.OnClose();
    }

    public override void Draw() {
        if (!Header("colormap-settings", false)) {
            return;
        }

        ImGui.Indent();

        if (ImGui.Button(Lang.Get("colormap-generate"), new Vector2 { X = 150f })) {
            _modal.Open();
        }

        ImGui.SameLine();
        ImGui.Text(Lang.Get("colormap-generate.text"));

        ImGui.SameLine();
        Editors.DrawHint(Lang.Get("colormap-generate.hint"));

        ImGui.Unindent();

        _modal.Draw();
    }
}
