using System;
using System.Numerics;
using ImGuiNET;
using livemap.client.util;
using livemap.common.util;

namespace livemap.client.gui.modal;

public class ColormapModal : Gui {
    private readonly ColormapGenerator _generator;

    private bool _colormapModalOpen;

    public event Action? Close;

    public ColormapModal(LiveMapClient client) {
        _generator = new ColormapGenerator(client);

        Close += OnClose;
    }

    public override void OnClose() {
        _colormapModalOpen = false;
        _generator.Cancel();
    }

    public void Open() {
        _colormapModalOpen = true;
        ImGui.OpenPopup(Lang.Get("colormap-generate"));
    }

    public override void Draw() {
        if (_colormapModalOpen) {
            Vector2 center = ImGui.GetWindowPos();
            center.X += ImGui.GetWindowSize().X / 2;
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0f));

            ImGui.GetStyle().Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0, 0, 0, 0.5f);
        }

        bool wasOpen = _colormapModalOpen;
        if (!ImGui.BeginPopupModal(Lang.Get("colormap-generate"), ref _colormapModalOpen, ImGuiWindowFlags.AlwaysAutoResize)) {
            if (wasOpen) {
                Close?.Invoke();
            }
            return;
        }

        Text(Lang.Get("colormap-generate.hint"), true);

        ImGui.Spacing();

        if (_generator.Running()) {
            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 275) * 0.5f);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.ProgressBar(_generator.Progress(), new Vector2(275, 0));
            ImGui.PopStyleVar();
        } else {
            ImGui.Text("");
        }

        ImGui.Spacing();

        ImGui.Spacing();
        ImGui.SameLine();
        Button(_generator.Running() ? "stop" : "start", 140, () => {
            if (_generator.Running()) {
                _generator.Cancel();
            } else {
                _generator.Run();
            }
        });
        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();
        Button(_generator.Running() ? "cancel" : "close", 140, () => {
            ImGui.CloseCurrentPopup();
            Close?.Invoke();
        });
        ImGui.SameLine();
        ImGui.Spacing();

        ImGui.Spacing();

        ImGui.EndPopup();
    }
}
