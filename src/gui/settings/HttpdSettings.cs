using System.Numerics;
using ImGuiNET;
using livemap.configuration;

namespace livemap.gui.settings;

public class HttpdSettings : Gui {
    public HttpdSettings(LiveMapClient client) : base(client) { }

    public override void Draw() {
        if (!Header("httpd-settings", true)) {
            return;
        }

        Config config = _client.Config!;

        bool enabled = config.Httpd.Enabled;
        int port = config.Httpd.Port;

        ImGui.Indent();

        ImGui.Dummy(new Vector2(116f, 1f));
        ImGui.SameLine();
        Input("httpd-enabled", name => ImGui.Checkbox("##" + name, ref enabled), 150f);
        Input("httpd-port", name => ImGui.InputInt("##" + name, ref port), 150f);

        ImGui.Unindent();

        config.Httpd.Enabled = enabled;
        config.Httpd.Port = port;
    }
}
