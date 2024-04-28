using System;
using System.Numerics;
using System.Reflection;
using ConfigLib;
using ImGuiNET;
using livemap.common.util;
using VSImGui;

namespace livemap.client.gui;

public abstract class Gui {
    public abstract void Draw();

    public virtual void OnClose() { }

    protected bool Header(string id, bool defaultOpen) {
        bool open = ImGui.CollapsingHeader($"{id.ToLang()}##{id}", defaultOpen ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None);
        Editors.DrawHint($"{$"{id}.hint".ToLang()}##{id}.hint");
        return open;
    }

    protected void Text(string text, uint? color) {
        Text(text, false, color);
    }

    protected void Text(string text, bool centered, uint? color = null) {
        Vector4? rgba = color == null
            ? null
            : new Vector4 {
                X = (float)(color >> 16 & 0xFF) / 0xFF,
                Y = (float)(color >> 8 & 0xFF) / 0xFF,
                Z = (float)(color >> 0 & 0xFF) / 0xFF,
                W = (float)(color >> 24 & 0xFF) / 0xFF
            };
        foreach (string line in text.Split("\n")) {
            if (line.Length > 0) {
                if (centered) {
                    float textWidth = ImGui.CalcTextSize(line).X;
                    ImGui.SetCursorPosX((ImGui.GetWindowSize().X - textWidth) * 0.5f);
                }
                if (rgba != null) {
                    ImGui.TextColored((Vector4)rgba, line);
                } else {
                    ImGui.Text(line);
                }
            } else {
                ImGui.NewLine();
            }
        }
    }

    protected void Button(string id, float width, Action action) {
        if (ImGui.Button($"{id.ToLang()}##{id}", new Vector2(width, 0))) {
            action.Invoke();
        }
    }

    protected void Input(string id, Action<string> input, float width) {
        using (new StyleApplier(new Style { SpacingItem = ImGui.GetStyle().ItemSpacing with { X = 0f } })) {
            ImGui.PushItemWidth(width);
            input.Invoke($"##{id}");
            ImGui.PopItemWidth();

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10f, 1f));

            ImGui.SameLine();
            ImGui.Text(id.ToLang());
            ImGui.SameLine();
            Editors.DrawHint($"{id}.hint");
        }
    }

    internal StyleEditor DebugEditor(ConfigLibModSystem configlib) {
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        object? guiManager = configlib.GetType()
            .GetField("_guiManager", flags)?
            .GetValue(configlib);
        object? configWindow = guiManager?.GetType()
            .GetField("_configWindow", flags)?
            .GetValue(guiManager);
        Style mainStyle = (Style)configWindow?.GetType()
            .GetField("_style", flags)?
            .GetValue(configWindow)!;
        return new StyleEditor(mainStyle);
    }
}