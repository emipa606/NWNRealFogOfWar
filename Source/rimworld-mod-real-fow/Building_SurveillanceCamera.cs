using System.Text;
using RimWorld;
using Verse;

namespace RimWorldRealFoW;

[StaticConstructorOnStartup]
public class Building_SurveillanceCamera : Building
{
    private MapComponentSeenFog mapComp;

    private CompPowerTrader powerComp;

    public bool isPowered()
    {
        return powerComp.PowerOn;
    }

    public override string GetInspectString()
    {
        var inspect = new StringBuilder();
        inspect.Append(base.GetInspectString());
        inspect.AppendInNewLine(mapComp.workingCameraConsole ? "Revealing".Translate() : "NoCameraConsole".Translate());

        return inspect.ToString();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        powerComp = GetComp<CompPowerTrader>();
        mapComp = map.getMapComponentSeenFog();
        mapComp.RegisterSurveillanceCamera(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);
        mapComp.DeregisterSurveillanceCamera(this);
    }
}