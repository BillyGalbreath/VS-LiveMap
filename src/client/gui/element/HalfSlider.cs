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

        // hardcode this because they're all the same size anyway. this isn't a shared library
        const float width = 150f;

        // draw an invisible button that allows other elements to overlap it
        // this is going to occupy the space as if it's a normal button
        Vector2 size = new(width, 26f);
        ImGui.SetNextItemAllowOverlap();
        ImGui.InvisibleButton("canvas", size, ImGuiButtonFlags.None);

        // get the exact screen coordinates of that button
        Vector2 minRect = ImGui.GetItemRectMin();
        Vector2 maxRect = ImGui.GetItemRectMax();

        // detect if mouse is hovering or grabbing
        int colorIndex = FindColorIndex(minRect, size);
        Vector4 color = style.Colors[colorIndex];

        // draw a colored rect the full width of where our slider will sit
        ImDrawListPtr list = ImGui.GetWindowDrawList();
        list.PushClipRect(minRect, maxRect);
        list.AddRectFilled(minRect, maxRect, color.ToColor(), 1f, ImDrawFlags.RoundCornersAll);
        list.PopClipRect();

        // we need the offset (half width minus width of grabber thing)
        // so we can push this to one side or the other (it's a half slider)
        const float offset = (width / 2f) - 10f;

        // stay on the same line, and push our cursor back to the beginning of the rect we drew
        // plus the offset if our slider is on the second half of this thing
        ImGui.SameLine();
        Vector2 pos = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(pos with { X = pos.X - width - style.ItemSpacing.X + (prePad ? offset : 0) });

        // now the fun part. draw the actual slider with all
        // the colors, so it looks natural with mouse actions
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

        // stay on same line but move back to the center of the actual slider
        ImGui.SameLine();
        pos = ImGui.GetCursorScreenPos();
        float halfWidth = ImGui.CalcTextSize($"{value}").X / 2f;
        ImGui.SetCursorScreenPos(pos with { X = pos.X - style.ItemSpacing.X - halfWidth - 10f - (prePad ? offset : 0) });

        // draw the text value in the center of the actual slider
        ImGui.Text($"{value}");

        // stay on same line and push the cursor back to the right after all the crap we just drew
        float x = ImGui.GetCursorScreenPos().X;
        ImGui.SameLine();
        pos = ImGui.GetCursorScreenPos();
        ImGui.SetCursorScreenPos(pos with { X = x + width + style.ItemSpacing.X + 2f });

        // finally draw the label text and hover hint
        ImGui.Text(id.ToLang());
        ImGui.SameLine();
        Editors.DrawHint($"{id}.hint");
    }

    private int FindColorIndex(Vector2 minRect, Vector2 size) {
        ImGuiIOPtr io = ImGui.GetIO();

        // get the current mouse coordinates relative to the coordinates of our rect
        Vector2 mouse = new() {
            X = (io.MousePos.X - minRect.X) / size.X,
            Y = (io.MousePos.Y - minRect.Y) / size.Y
        };

        // check if mouse is hovering our rect or button is pressed down
        bool hovered = mouse.X is > 0 and < 1 && mouse.Y is > 0 and < 1;
        bool isDown = io.MouseDown[0];

        // do some super hard calculations to determine if the mouse clicked on our slider to initiate a grab
        if (hovered) {
            if (isDown && !_wasDown) {
                _active = true;
                _wasDown = true;
            }
        }

        // mouse isn't down, so we know it's not an active grab
        if (!isDown) {
            _active = false;
            _wasDown = false;
        }

        // return the active color index if active
        if (_active) {
            return (int)ImGuiCol.FrameBgActive;
        }

        // return the hover color index is hovered
        if (hovered) {
            return (int)ImGuiCol.FrameBgHovered;
        }

        // or just return the normal color index
        return (int)ImGuiCol.FrameBg;
    }
}
