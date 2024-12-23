using RimWorldRealFoW.Utils;
using Verse;
using Verse.AI;

namespace RimWorldRealFoW.Detours;

public static class _HaulAIUtility
{
    public static bool HaulToStorageJob_Prefix(Pawn p, Thing t, Job __result)
    {
        return !(p.Faction is { IsPlayer: true } && !t.fowIsVisible());
    }
}