using HarmonyLib;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _RoofGrid
{
    public static void GetCellBool_Postfix(int index, ref TerrainGrid __instance, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        var value = Traverse.Create(__instance).Field("map").GetValue<Map>();
        var mapComponentSeenFog = value.getMapComponentSeenFog();
        if (mapComponentSeenFog != null)
        {
            __result = mapComponentSeenFog.knownCells[index];
        }
    }
}