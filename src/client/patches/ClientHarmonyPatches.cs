using HarmonyLib;
using livemap.common.patches;

namespace livemap.client.patches;

public sealed class ClientHarmonyPatches : HarmonyPatches {
    protected override void Init(Harmony harmony) {
        _ = new GameCalendarPatches(harmony);
    }
}
