using System.Numerics;
using ImGuiNET;
using livemap.gui.modal;
using livemap.util;
using VSImGui;

namespace livemap.gui.settings;

public class ColormapSettings : Gui {
    private readonly ColormapModal _modal;

    public ColormapSettings(LiveMapClient client) : base(client) {
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

        using (new StyleApplier(new Style { SpacingItem = ImGui.GetStyle().ItemSpacing with { X = 4f } })) {
            if (ImGui.Button("colormap-generate".ToLang(), new Vector2 { X = 150f })) {
                _modal.Open();
            }

            ImGui.SameLine();
            ImGui.Text("colormap-generate.text".ToLang());

            ImGui.SameLine();
            Editors.DrawHint("colormap-generate.hint".ToLang());
        }

        ImGui.Unindent();

        _modal.Draw();
    }
}
