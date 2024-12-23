using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _MainTabWindow_Wildlife
{
    public static bool get_Pawns_Prefix(ref IEnumerable<Pawn> __result)
    {
        __result = Find.CurrentMap.mapPawns.AllPawns.Where(p => p.Spawned && (p.Faction == null
                                                                              || p.Faction == Faction.OfInsects)
                                                                          && p.AnimalOrWildMan()
                                                                          && !p.Position.Fogged(p.Map)
                                                                          && (p.fowIsVisible() ||
                                                                              RFOWSettings.wildLifeTabVisible));
        return false;
    }
}