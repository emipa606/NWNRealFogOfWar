using Verse;

namespace RimWorldRealFoW.Detours;

public static class _EnvironmentStatsDrawer
{
    public static void ShouldShowWindowNow_Postfix(ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        var currentMap = Find.CurrentMap;
        var mapComponentSeenFog = currentMap.getMapComponentSeenFog();
        __result = mapComponentSeenFog == null ||
                   mapComponentSeenFog.knownCells[currentMap.cellIndices.CellToIndex(UI.MouseCell())];
    }
}