using HarmonyLib;
using LiveMap.Common.Patches;

namespace LiveMap.Client.Patches;

public sealed class ClientHarmonyPatches : HarmonyPatches {
    protected override void Init(Harmony harmony) {
        _ = new GameCalendarPatches(harmony);
    }
}
