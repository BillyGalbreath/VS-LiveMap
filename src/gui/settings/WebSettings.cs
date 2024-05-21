using System.Numerics;
using ImGuiNET;
using livemap.configuration;
using livemap.tile;
using livemap.util;
using VSImGui;

namespace livemap.gui.settings;

public class WebSettings : Gui {
    public WebSettings(LiveMapClient client) : base(client) { }

    public override void Draw() {
        if (!Header("web-settings", false)) {
            return;
        }

        Config config = _client.Config!;

        string path = config.Web.Path;
        bool @readonly = config.Web.ReadOnly;
        TileType tiletype = config.Web.TileType;
        int tilequality = config.Web.TileQuality;

        ImGui.Indent();

        Input("web-path", name => ImGui.InputText("##" + name, ref path, (uint)short.MaxValue), 150f);

        ImGui.Dummy(new Vector2(116f, 1f));
        ImGui.SameLine();
        Input("web-readonly", name => ImGui.Checkbox("##" + name, ref @readonly), 150f);

        using (new StyleApplier(new Style { SpacingItem = ImGui.GetStyle().ItemSpacing with { X = 0f } })) {
            ImGui.PushItemWidth(150f);
            if (ImGui.BeginCombo("web-tiletype".ToLang(), tiletype.Type, 0)) {
                foreach ((string? key, TileType? type) in TileType.Types) {
                    bool isSelected = type == tiletype;
                    if (ImGui.Selectable(key, isSelected)) {
                        tiletype = type;
                    }
                    if (isSelected) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(4f, 1f));
            ImGui.SameLine();
            Editors.DrawHint("web-tiletype.hint".ToLang());
        }

        Input("web-tilequality", name => ImGui.SliderInt("##" + name, ref tilequality, 0, 100), 150f);

        ImGui.Unindent();

        config.Web.Path = path;
        config.Web.ReadOnly = @readonly;
        config.Web.TileType = tiletype;
        config.Web.TileQuality = tilequality;
    }
}
