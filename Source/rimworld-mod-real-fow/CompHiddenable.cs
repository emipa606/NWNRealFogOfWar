using RimWorld;
using Verse;

namespace RimWorldRealFoW;

public class CompHiddenable : ThingSubComp
{
    public bool hidden;

    private IntVec3 lastPosition = IntVec3.Invalid;

    private Map map;

    private MapComponentSeenFog mapComp;

    public void hide()
    {
        if (hidden)
        {
            return;
        }

        hidden = true;
        if (parent.def.drawerType != DrawerType.MapMeshOnly)
        {
            parent.Map.dynamicDrawManager.DeRegisterDrawable(parent);
        }

        var hasTooltip = parent.def.hasTooltip;
        if (hasTooltip)
        {
            parent.Map.tooltipGiverList.Notify_ThingDespawned(parent);
        }

        var selector = Find.Selector;
        if (selector.IsSelected(parent))
        {
            selector.Deselect(parent);
        }
        FoW_AudioCache.StopAll(parent); // Find's Thing in dictionary and removes the associated audio sustainer.
        updateMeshes();
    }

    public void show()
    {
        if (!hidden)
        {
            return;
        }

        hidden = false;
        if (parent.def.drawerType != DrawerType.MapMeshOnly)
        {
            parent.Map.dynamicDrawManager.RegisterDrawable(parent);
        }

        var hasTooltip = parent.def.hasTooltip;
        if (hasTooltip)
        {
            parent.Map.tooltipGiverList.Notify_ThingSpawned(parent);
        }

        updateMeshes();
    }

    private void updateMeshes()
    {
        if (map != parent.Map)
        {
            map = parent.Map;
            mapComp = map.getMapComponentSeenFog();
        }

        if (mapComp is not { initialized: true })
        {
            return;
        }

        foreach (var intVec in parent.OccupiedRect().Cells)
        {
            if (intVec.InBounds(map))
            {
                map.mapDrawer.MapMeshDirty(intVec, MapMeshFlagDefOf.Things | MapMeshFlagDefOf.Buildings |
                                                   MapMeshFlagDefOf.GroundGlow |
                                                   MapMeshFlagDefOf.Terrain | MapMeshFlagDefOf.Roofs |
                                                   MapMeshFlagDefOf.Snow | MapMeshFlagDefOf.Pollution |
                                                   MapMeshFlagDefOf.Zone | MapMeshFlagDefOf.PowerGrid |
                                                   MapMeshFlagDefOf.BuildingsDamage | MapMeshFlagDefOf.Gas);
            }
        }
    }
}