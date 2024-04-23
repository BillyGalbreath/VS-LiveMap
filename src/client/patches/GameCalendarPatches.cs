using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

namespace livemap.client.patches;

public class GameCalendarPatches {
    public static BlockPos? OverridePos { get; set; }

    internal GameCalendarPatches(Harmony harmony) {
        _ = new YearRelPatch(harmony);
    }

    private class YearRelPatch {
        public YearRelPatch(Harmony harmony) {
            harmony.Patch(typeof(GameCalendar).GetProperty("YearRel", BindingFlags.Instance | BindingFlags.Public)!.GetGetMethod(),
                prefix: GetType().GetMethod("Prefix"));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public static bool Prefix(IGameCalendar __instance, ref float __result) {
            if (OverridePos == null) {
                return true;
            }

            __result = __instance.GetHemisphere(OverridePos) == EnumHemisphere.North ? 0.6f : 0.1f;
            return false;
        }
    }
}
