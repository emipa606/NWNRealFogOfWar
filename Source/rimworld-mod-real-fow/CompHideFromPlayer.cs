using RimWorld;
using Verse;

namespace RimWorldRealFoW;

public class CompHideFromPlayer : ThingSubComp
{
    private static readonly IntVec3 iv3Invalid = IntVec3.Invalid;

    private static readonly Rot4 r4Invalid = Rot4.Invalid;

    private bool calculated;

    private CompHiddenable compHiddenable;

    private FogGrid fogGrid;

    private bool isOneCell;

    private bool isPawn;

    private bool isSaveable;

    private IntVec3 lastPosition;

    private Rot4 lastRotation;
    private int lastUpdateTick;

    private Map map;

    private MapComponentSeenFog mapCompSeenFog;

    private bool saveCompressible;

    private bool seenByPlayer;

    private bool setupDone;

    private IntVec2 size;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        setupDone = true;
        calculated = false;
        lastPosition = iv3Invalid;
        lastRotation = r4Invalid;
        isPawn = parent.def.category == ThingCategory.Pawn;
        size = parent.def.size;
        isOneCell = size is { z: 1, x: 1 };
        isSaveable = parent.def.isSaveable;
        saveCompressible = parent.def.saveCompressible;
        compHiddenable = mainComponent.compHiddenable;
        lastUpdateTick = Find.TickManager.TicksGame;
        updateVisibility(false);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref seenByPlayer, "seenByPlayer");
    }

    public override void ReceiveCompSignal(string signal)
    {
        base.ReceiveCompSignal(signal);
        updateVisibility(false);
    }

    public override void CompTick()
    {
        base.CompTick();
        var tickGame = Find.TickManager.TicksGame;
        if (tickGame - lastUpdateTick != 12)
        {
            return;
        }

        lastUpdateTick = tickGame;
        updateVisibility(false);
    }

    public void forceSeen()
    {
        seenByPlayer = true;
        updateVisibility(true, true);
    }

    public void updateVisibility(bool forceCheck, bool forceUpdate = false)
    {
        if (!setupDone || Current.ProgramState == ProgramState.MapInitializing)
        {
            return;
        }

        Thing thingParent = parent;
        var position = thingParent.Position;
        var rotation = thingParent.Rotation;
        if (thingParent is not { Spawned: true, Map: not null } || position == iv3Invalid ||
            !isOneCell && rotation == r4Invalid)
        {
            return;
        }

        if (map != thingParent.Map)
        {
            map = thingParent.Map;
            fogGrid = map.fogGrid;
            mapCompSeenFog = thingParent.Map.getMapComponentSeenFog();
        }
        else
        {
            if (mapCompSeenFog == null)
            {
                mapCompSeenFog = thingParent.Map.getMapComponentSeenFog();
            }
        }

        if (mapCompSeenFog == null)
        {
            return;
        }

        if (!forceCheck && calculated && position == lastPosition &&
            (isOneCell || rotation == lastRotation))
        {
            return;
        }

        calculated = true;
        lastPosition = position;
        lastRotation = rotation;
        if (mapCompSeenFog == null || fogGrid.IsFogged(lastPosition))
        {
            return;
        }

        if (isSaveable && !saveCompressible)
        {
            if (thingParent.Faction is not { IsPlayer: true })
            {
                if (isPawn && !hasPartShownToPlayer())
                {
                    compHiddenable.hide();
                }
                else
                {
                    if (!isPawn && !seenByPlayer && !hasPartShownToPlayer())
                    {
                        compHiddenable.hide();
                    }
                    else
                    {
                        seenByPlayer = true;
                        compHiddenable.show();
                    }
                }
            }
            else
            {
                seenByPlayer = true;
                compHiddenable.show();
            }
        }
        else
        {
            if (!forceUpdate && seenByPlayer || !hasPartShownToPlayer())
            {
                return;
            }

            seenByPlayer = true;
            compHiddenable.show();
        }
    }

    private bool hasPartShownToPlayer()
    {
        var ofPlayer = Faction.OfPlayer;
        bool result;
        if (isOneCell)
        {
            result = mapCompSeenFog.isShown(ofPlayer, lastPosition.x, lastPosition.z);
        }
        else
        {
            var cellRect = GenAdj.OccupiedRect(lastPosition, lastRotation, size);
            for (var i = cellRect.minX; i <= cellRect.maxX; i++)
            {
                for (var j = cellRect.minZ; j <= cellRect.maxZ; j++)
                {
                    if (mapCompSeenFog.isShown(ofPlayer, i, j))
                    {
                        return true;
                    }
                }
            }

            result = false;
        }

        return result;
    }
}