using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using Vintagestory.Common;

namespace LiveMap.Client.Patches;

public class GameCalendarPatches {
    public static bool Override { get; set; }

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
        public static bool Prefix(ref float __result) {
            if (!Override) {
                return true;
            }

            __result = 0.5f;
            return false;
        }
    }
}
