using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class ReservationUtility
{
    public static void CanReserve_Postfix(this Verse.Pawn p, ref bool __result, LocalTargetInfo target)
    {
        if (__result && p.Faction is { IsPlayer: true } && target.HasThing &&
            target.Thing.def.category != ThingCategory.Pawn)
        {
            __result = target.Thing.FowIsVisible();
        }
    }

    public static void CanReserveAndReach_Postfix(this Verse.Pawn p, ref bool __result, LocalTargetInfo target)
    {
        if (__result && p.Faction is { IsPlayer: true } && target.HasThing &&
            target.Thing.def.category != ThingCategory.Pawn)
        {
            __result = target.Thing.FowIsVisible();
        }
    }
}