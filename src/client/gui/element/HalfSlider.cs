using System.Numerics;
using ImGuiNET;
using livemap.common.util;
using VSImGui;

namespace livemap.client.gui.element;

public class HalfSlider {
    private bool _active;
    private bool _wasDown;

    public void Draw(string id, ref int value, int min, int max, bool prePad) {
        ImGuiStylePtr style = ImGui.GetStyle();

        const float width = 150f;

        Vector2 size = new(width, 26f);
        ImGui.SetNextItemAllowOverlap();
        ImGui.InvisibleButton("canvas", size, ImGuiButtonFlags.None);

        Vector2 minRect = ImGui.GetItemRectMin();
        Vector2 maxRect = ImGui.GetItemRectMax();

        int colorIndex = FindColorIndex(minRect, size);
        Vector4 color = style.Colors[colorIndex];

        ImDrawListPtr list = ImGui.GetWindowDrawList();
        list.PushClipRect(minRect, maxRect);
        list.AddRectFilled(minRect, maxRect, color.ToColor(), 1f, ImDrawFlags.RoundCornersAll);
        list.PopClipRect();

        const float offset = (width / 2f) - 10f;

        ImGui.SameLine();
        Vector2 pos = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(pos with { X = pos.X - width - style.ItemSpacing.X + (prePad ? offset : 0) });

        ImGui.PushItemWidth((width / 2f) + 10f);
        Vector4 text = style.Colors[(int)ImGuiCol.Text];
        Vector4 frameBg = style.Colors[(int)ImGuiCol.FrameBg];
        style.Colors[(int)ImGuiCol.Text] = Vector4.Zero;
        style.Colors[(int)ImGuiCol.FrameBg] = Vector4.Zero;
        style.Colors[colorIndex] = Vector4.Zero;
        ImGui.SliderInt($"##{id}", ref value, min, max);
        style.Colors[colorIndex] = color;
        style.Colors[(int)ImGuiCol.FrameBg] = frameBg;
        style.Colors[(int)ImGuiCol.Text] = text;
        ImGui.PopItemWidth();

        ImGui.SameLine();
        pos = ImGui.GetCursorScreenPos();
        float halfWidth = ImGui.CalcTextSize($"{value}").X / 2f;
        ImGui.SetCursorScreenPos(pos with { X = pos.X - style.ItemSpacing.X - halfWidth - 10f - (prePad ? offset : 0) });

        ImGui.Text($"{value}");

        float x = ImGui.GetCursorScreenPos().X;
        ImGui.SameLine();
        pos = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(pos with { X = x + width + style.ItemSpacing.X + 2f });

        ImGui.Text(id.ToLang());
        ImGui.SameLine();
        Editors.DrawHint($"{id}.hint");
    }

    private int FindColorIndex(Vector2 minRect, Vector2 size) {
        ImGuiIOPtr io = ImGui.GetIO();
        Vector4 mouse = new() {
            X = (io.MousePos.X - minRect.X) / size.X,
            Y = (io.MousePos.Y - minRect.Y) / size.Y,
            Z = io.MouseDownDuration[0],
            W = io.MouseDownDuration[1]
        };

        bool hovered = mouse.X is > 0 and < 1 && mouse.Y is > 0 and < 1;
        bool isDown = io.MouseDown[0];

        if (hovered) {
            if (isDown && !_wasDown) {
                _active = true;
                _wasDown = true;
            }
        }

        if (!isDown) {
            _active = false;
            _wasDown = false;
        }

        if (_active) {
            return (int)ImGuiCol.FrameBgActive;
        }
        if (hovered) {
            return (int)ImGuiCol.FrameBgHovered;
        }
        return (int)ImGuiCol.FrameBg;
    }
}
