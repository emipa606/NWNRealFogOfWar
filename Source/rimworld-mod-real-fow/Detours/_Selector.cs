using RimWorld.Planet;
using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _Selector
{
    public static bool Select_Prefix(object obj)
    {
        var thing = obj as Thing;
        var pawn = obj as Pawn;
        return !(thing is { Destroyed: false } && (pawn == null || !pawn.IsWorldPawn()) && !thing.fowIsVisible());
    }
}