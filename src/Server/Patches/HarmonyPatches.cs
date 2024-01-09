using HarmonyLib;
using Vintagestory.API.Common;

namespace LiveMap.Server.Patches;

public class HarmonyPatches {
    private readonly string modId;

    private Harmony? harmony;

    public HarmonyPatches(ModSystem mod) {
        modId = mod.Mod.Info.ModID;
        harmony = new Harmony(modId);

        _ = new PrivilegePatches(harmony);
    }

    public void Dispose() {
        harmony?.UnpatchAll(modId);
        harmony = null;
    }
}
