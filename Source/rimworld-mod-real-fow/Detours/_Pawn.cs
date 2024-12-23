using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours;

internal static class _Pawn
{
    public static bool DrawGUIOverlay_Prefix(Pawn __instance)
    {
        return !(__instance.Spawned && !__instance.fowIsVisible());
    }
}