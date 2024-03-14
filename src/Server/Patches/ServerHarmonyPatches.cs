using HarmonyLib;
using LiveMap.Common.Patches;

namespace LiveMap.Server.Patches;

public sealed class ServerHarmonyPatches : HarmonyPatches {
    protected override void Init(Harmony harmony) {
        _ = new PrivilegePatches(harmony);
    }
}
