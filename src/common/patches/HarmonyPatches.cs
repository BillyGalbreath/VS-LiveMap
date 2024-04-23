using HarmonyLib;

namespace livemap.common.patches;

public abstract class HarmonyPatches {
    private Harmony? _harmony = new(LiveMapMod.Id);

    public void Init() {
        Init(_harmony!);
    }

    protected abstract void Init(Harmony harmony);

    public void Dispose() {
        _harmony?.UnpatchAll(LiveMapMod.Id);
        _harmony = null;
    }
}
