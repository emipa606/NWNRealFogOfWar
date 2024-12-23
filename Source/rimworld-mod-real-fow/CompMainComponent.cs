using Verse;

namespace RimWorldRealFoW;

public class CompMainComponent : ThingComp
{
    public static readonly CompProperties COMP_DEF = new CompProperties(typeof(CompMainComponent));
    public CompComponentsPositionTracker compComponentsPositionTracker;
    public CompFieldOfViewWatcher compFieldOfViewWatcher;
    public CompHiddenable compHiddenable;
    public CompHideFromPlayer compHideFromPlayer;
    public CompViewBlockerWatcher compViewBlockerWatcher;
    private bool setup;

    private void performSetup()
    {
        if (setup)
        {
            return;
        }

        setup = true;

        var category = parent.def.category;

        compComponentsPositionTracker = new CompComponentsPositionTracker
        {
            parent = parent,
            mainComponent = this
        };

        compHiddenable = new CompHiddenable
        {
            parent = parent,
            mainComponent = this
        };

        compHideFromPlayer = new CompHideFromPlayer
        {
            parent = parent,
            mainComponent = this
        };

        if (category == ThingCategory.Building)
        {
            compViewBlockerWatcher = new CompViewBlockerWatcher
            {
                parent = parent,
                mainComponent = this
            };
        }

        if (
            category == ThingCategory.Pawn
            || category == ThingCategory.Building
            //||category == ThingCategory.Projectile
        )
        {
            compFieldOfViewWatcher = new CompFieldOfViewWatcher
            {
                parent = parent,
                mainComponent = this
            };
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.PostSpawnSetup(respawningAfterLoad);

        compHiddenable.PostSpawnSetup(respawningAfterLoad);

        compHideFromPlayer.PostSpawnSetup(respawningAfterLoad);

        compViewBlockerWatcher?.PostSpawnSetup(respawningAfterLoad);
        compFieldOfViewWatcher?.PostSpawnSetup(respawningAfterLoad);
    }

    public override void CompTick()
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.CompTick();
        compHiddenable.CompTick();
        compHideFromPlayer.CompTick();
        compViewBlockerWatcher?.CompTick();
        compFieldOfViewWatcher?.CompTick();
    }

    public override void CompTickRare()
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.CompTickRare();
        compHiddenable.CompTickRare();
        compHideFromPlayer.CompTickRare();
        compViewBlockerWatcher?.CompTickRare();
        compFieldOfViewWatcher?.CompTickRare();
    }

    public override void ReceiveCompSignal(string signal)
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.ReceiveCompSignal(signal);
        compHiddenable.ReceiveCompSignal(signal);
        compHideFromPlayer.ReceiveCompSignal(signal);
        compViewBlockerWatcher?.ReceiveCompSignal(signal);
        compFieldOfViewWatcher?.ReceiveCompSignal(signal);
    }

    public override void PostDeSpawn(Map map)
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.PostDeSpawn(map);
        compHiddenable.PostDeSpawn(map);
        compHideFromPlayer.PostDeSpawn(map);
        compViewBlockerWatcher?.PostDeSpawn(map);
        compFieldOfViewWatcher?.PostDeSpawn(map);
    }

    public override void PostExposeData()
    {
        if (!setup)
        {
            performSetup();
        }

        compComponentsPositionTracker.PostExposeData();
        compHiddenable.PostExposeData();
        compHideFromPlayer.PostExposeData();

        compViewBlockerWatcher?.PostExposeData();
        compFieldOfViewWatcher?.PostExposeData();
        if (!Scribe.saver.savingForDebug)
        {
            return;
        }

        var hasCompComponentsPositionTracker = compComponentsPositionTracker != null;
        var hasCompHiddenable = compHiddenable != null;
        var hasCompHideFromPlayer = compHideFromPlayer != null;
        var hasCompViewBlockerWatcher = compViewBlockerWatcher != null;
        var hasCompFieldOfViewWatcher = compFieldOfViewWatcher != null;
        Scribe_Values.Look(ref hasCompComponentsPositionTracker, "hasCompComponentsPositionTracker");
        Scribe_Values.Look(ref hasCompHiddenable, "hasCompHiddenable");
        Scribe_Values.Look(ref hasCompHideFromPlayer, "hasCompHideFromPlayer");
        Scribe_Values.Look(ref hasCompViewBlockerWatcher, "hasCompViewBlockerWatcher");
        Scribe_Values.Look(ref hasCompFieldOfViewWatcher, "hasCompFieldOfViewWatcher");
    }
}