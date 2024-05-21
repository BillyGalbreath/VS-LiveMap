using ImGuiNET;
using livemap.configuration;
using livemap.gui.element;

namespace livemap.gui.settings;

public class ZoomSettings : Gui {
    private readonly HalfSlider _sliderMaxIn;
    private readonly HalfSlider _sliderMaxOut;

    public ZoomSettings(LiveMapClient client) : base(client) {
        _sliderMaxIn = new HalfSlider();
        _sliderMaxOut = new HalfSlider();
    }

    public override void Draw() {
        if (!Header("zoom-settings", false)) {
            return;
        }

        Config config = _client.Config!;

        int maxIn = config.Zoom.MaxIn;
        int maxOut = config.Zoom.MaxOut;
        int def = config.Zoom.Default;

        ImGui.Indent();

        _sliderMaxIn.Draw("zoom.max-in", ref maxIn, -10, 0, false);
        _sliderMaxOut.Draw("zoom.max-out", ref maxOut, 0, 10, true);
        Input("zoom.default", label => ImGui.SliderInt(label, ref def, -10, 10), 150f);

        ImGui.Unindent();

        Clamp(config, maxIn, maxOut, def);
    }

    private static void Clamp(Config config, int maxIn, int maxOut, int def) {
        if (config.Zoom.MaxIn != maxIn) {
            if (maxIn > 0) {
                maxIn = 0;
            }
            if (def < maxIn) {
                def = maxIn;
            }
            config.Zoom.MaxIn = maxIn;
        }

        if (config.Zoom.MaxOut != maxOut) {
            if (maxOut < 0) {
                maxOut = 0;
            }
            if (def > maxOut) {
                def = maxOut;
            }
            config.Zoom.MaxOut = maxOut;
        }

        // ReSharper disable once InvertIf
        if (config.Zoom.Default != def) {
            if (def < maxIn) {
                config.Zoom.MaxIn = def;
            } else if (def > maxOut) {
                config.Zoom.MaxOut = def;
            }
            config.Zoom.Default = def;
        }
    }
}
