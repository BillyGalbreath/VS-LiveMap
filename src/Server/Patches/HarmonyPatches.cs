using HarmonyLib;

namespace LiveMap.Server.Patches;

public sealed class HarmonyPatches {
    private Harmony? _harmony;

    public HarmonyPatches() {
        _harmony = new Harmony(LiveMapMod.Id);

        _ = new PrivilegePatches(_harmony);
    }

    public void Dispose() {
        _harmony?.UnpatchAll(LiveMapMod.Id);
        _harmony = null;
    }
}
