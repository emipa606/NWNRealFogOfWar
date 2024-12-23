using RimWorld;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _Designation
{
    public static void Notify_Added_Postfix(ref Designation __instance)
    {
        if (__instance.def != DesignationDefOf.Mine || __instance.target.HasThing)
        {
            return;
        }

        var mapComponentSeenFog = __instance.designationManager.map.getMapComponentSeenFog();
        mapComponentSeenFog?.RegisterMineDesignation(__instance);
    }

    public static void Notify_Removing_Postfix(ref Designation __instance)
    {
        if (__instance.def != DesignationDefOf.Mine || __instance.target.HasThing)
        {
            return;
        }

        var mapComponentSeenFog = __instance.designationManager.map.getMapComponentSeenFog();
        mapComponentSeenFog?.DeregisterMineDesignation(__instance);
    }
}