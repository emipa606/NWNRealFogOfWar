using RimWorld;
using RimWorldRealFoW.Utils;
using UnityEngine;
using Verse;

namespace RimWorldRealFoW.Detours;

internal static class _Verb
{
    internal static void CanHitCellFromCellIgnoringRange_Postfix(this Verb __instance, ref bool __result,
        IntVec3 sourceSq, IntVec3 targetLoc, bool includeCorners = false)
    {
        if (__result && __instance.verbProps.requireLineOfSight)
        {
            __result = __instance.caster.Faction != null && SeenByFaction(__instance.caster, targetLoc) ||
                       fovLineOfSight(sourceSq, targetLoc, __instance.caster);
        }
    }

    private static bool SeenByFaction(Thing thing, IntVec3 targetLoc)
    {
        var mapComponentSeenFog = thing.Map.getMapComponentSeenFog();
        return mapComponentSeenFog == null || mapComponentSeenFog.isShown(thing.Faction, targetLoc);
    }

    private static bool fovLineOfSight(IntVec3 sourceSq, IntVec3 targetLoc, Thing thing)
    {
        var compMannable = thing.TryGetComp<CompMannable>();
        if (compMannable != null)
        {
            thing = compMannable.ManningPawn;
            sourceSq += thing.Position - thing.InteractionCell;
        }

        bool result;
        if (thing is not Pawn)
        {
            result = true;
        }
        else
        {
            var mapComponentSeenFog = thing.Map.getMapComponentSeenFog();
            var compMainComponent = (CompMainComponent)thing.TryGetCompLocal(CompMainComponent.COMP_DEF);
            var compFieldOfViewWatcher = compMainComponent.compFieldOfViewWatcher;
            var num = Mathf.RoundToInt(compFieldOfViewWatcher.CalcPawnSightRange(sourceSq, true,
                !thing.Position.AdjacentToCardinal(sourceSq)));
            if (!sourceSq.InHorDistOf(targetLoc, num))
            {
                result = false;
            }
            else
            {
                var intVec = targetLoc - sourceSq;
                byte specificOctant;
                if (intVec.x >= 0)
                {
                    if (intVec.z >= 0)
                    {
                        specificOctant = intVec.x >= intVec.z ? (byte)0 : (byte)1;
                    }
                    else
                    {
                        specificOctant = intVec.x >= -intVec.z ? (byte)7 : (byte)6;
                    }
                }
                else
                {
                    if (intVec.z >= 0)
                    {
                        specificOctant = -intVec.x >= intVec.z ? (byte)3 : (byte)2;
                    }
                    else
                    {
                        specificOctant = -intVec.x >= -intVec.z ? (byte)4 : (byte)5;
                    }
                }

                var map = thing.Map;
                var array = new bool[1];
                ShadowCaster.computeFieldOfViewWithShadowCasting(sourceSq.x, sourceSq.z, num,
                    mapComponentSeenFog.viewBlockerCells, map.Size.x, map.Size.z, false, null, null, null, array, 0, 0,
                    0, null, 0, 0, 0, 0, 0, specificOctant, targetLoc.x, targetLoc.z);
                result = array[0];
            }
        }

        return result;
    }
}