using HarmonyLib;

namespace LiveMap.Server.Patches;

public sealed class HarmonyPatches {
    private Harmony? harmony;

    public HarmonyPatches() {
        harmony = new Harmony(LiveMapMod.Id);

        _ = new PrivilegePatches(harmony);
    }

    public void Dispose() {
        harmony?.UnpatchAll(LiveMapMod.Id);
        harmony = null;
    }
}
