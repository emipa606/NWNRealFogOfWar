using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _FertilityGrid
{
    public static void CellBoolDrawerGetBoolInt_Postfix(int index, ref FertilityGrid __instance, ref bool __result)
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