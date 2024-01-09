using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Server;

namespace LiveMap.Server.Patches;

public class PrivilegePatches {
    protected internal PrivilegePatches(Harmony harmony) {
        _ = new AllCodesPatch(harmony);
    }

    private class AllCodesPatch {
        public AllCodesPatch(Harmony harmony) {
            harmony.Patch(typeof(Privilege).GetMethod("AllCodes", BindingFlags.Static | BindingFlags.Public),
                postfix: GetType().GetMethod("Postfix"));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public static void Postfix(ref string[] __result) {
            int index = __result.Length;
            Array.Resize(ref __result, index + 1);
            __result[index] = "livemap.admin";
        }
    }
}
