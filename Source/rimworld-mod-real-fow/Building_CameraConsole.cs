using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWorldRealFoW;

[StaticConstructorOnStartup]
public class Building_CameraConsole : Building
{
    private readonly Graphic[] workingGraphics = new Graphic[13];
    private CompBreakdownable breakdownableComp;

    private Graphic censorGraphic;

    private int lastTick;

    private MapComponentSeenFog mapComp;

    private CompPowerTrader powerComp;


    public bool Manned => Find.TickManager.TicksGame < lastTick + 100;

    public bool WorkingNow => FlickUtility.WantsToBeOn(this) && (powerComp == null || powerComp.PowerOn) &&
                              breakdownableComp is not { BrokenDown: true };

    public override string GetInspectString()
    {
        var inspect = new StringBuilder();
        inspect.Append(base.GetInspectString());
        if (mapComp != null)
        {
            inspect.AppendInNewLine("CameraCount".Translate() + ": " + mapComp.SurveillanceCameraCount());
        }

        return inspect.ToString();
    }

    public static bool NeedWatcher()
    {
        //return mapComp.SurveillanceCameraCount() >= 1;
        //Turret need the console to work so just keep it like this
        return true;
    }

    private void drawOverLay()
    {
        if (!Manned)
        {
            return;
        }

        var cameraCount = Mathf.Min(mapComp.SurveillanceCameraCount(), 12);
        workingGraphics[cameraCount] ??= GraphicDatabase.Get(
            def.graphicData.graphicClass,
            $"{def.graphicData.texPath}_FX{cameraCount}",
            ShaderDatabase.MoteGlow,
            def.graphicData.drawSize,
            DrawColor,
            DrawColorTwo
        );

        workingGraphics[cameraCount].Draw(DrawPos + new Vector3(0f, 1f, 0f), Rotation, this);
        if (!RfowSettings.CensorMode)
        {
            return;
        }

        censorGraphic ??= GraphicDatabase.Get(
            def.graphicData.graphicClass,
            $"{def.graphicData.texPath}_FX_Censor",
            ShaderDatabase.MoteGlow,
            def.graphicData.drawSize,
            DrawColor,
            DrawColorTwo
        );

        censorGraphic.Draw(DrawPos + new Vector3(0f, 1f, 0f), Rotation, this);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        //((ThingWithComps)this).Draw();
        base.DrawAt(drawLoc, flip);
        drawOverLay();
    }

    public void Used()
    {
        lastTick = Find.TickManager.TicksGame;
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        powerComp = GetComp<CompPowerTrader>();
        breakdownableComp = GetComp<CompBreakdownable>();
        mapComp = map.GetMapComponentSeenFog();
        mapComp.RegisterCameraConsole(this);
        //12 Possible graphic so
        /*
        for (int i = 0; i <= 12; i++)
        {
            workingGraphics.Add(GraphicDatabase.Get(
                this.def.graphicData.graphicClass,
                this.def.graphicData.texPath + "_FX" + (i).ToString(), ShaderDatabase.MoteGlow,
                this.def.graphicData.drawSize,
                this.DrawColor,
                this.DrawColorTwo
                ));
        }*/
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);
        mapComp.DeregisterCameraConsole(this);
    }
}