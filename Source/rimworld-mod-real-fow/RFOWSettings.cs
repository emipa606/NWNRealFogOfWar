using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorldRealFoW;

public class RFOWSettings : ModSettings
{
    public enum FogAlpha
    {
        Black = 255,
        NearlyBlack = 210,
        VeryVeryVeryDark = 180,
        VeryVeryDark = 150,
        VeryDark = 120,
        Dark = 100,
        Medium = 80,
        Light = 60
    }

    public enum FogFadeSpeedEnum
    {
        Slow = 5,
        Medium = 20,
        Fast = 40,
        Disabled = 100
    }

    public static Vector2 scrollPosition = Vector2.zero;

    public static FogFadeSpeedEnum fogFadeSpeed = FogFadeSpeedEnum.Medium;

    public static FogAlpha fogAlpha = FogAlpha.Medium;

    public static int baseViewRange = 60;
    public static float baseHearingRange = 10;

    public static float buildingVisionModifier = 1;

    public static float turretVisionModifier = 0.7f;

    public static float animalVisionModifier = 0.5f;

    public static bool hideSpeakBubble;

    public static bool aiSmart;
    public static bool censorMode;
    public static bool needWatcher = true;
    public static bool hideThreatBig;
    public static bool hideThreatSmall;
    public static bool hideEventPositive;
    public static bool hideEventNegative;
    public static bool hideEventNeutral;
    public static bool prisonerGiveVision;
    public static bool allyGiveVision;
    public static bool mapRevealAtStart;
    public static bool wildLifeTabVisible = true;
    public static bool needMemoryStorage = true;

    public static void DoSettingsWindowContents(Rect rect)
    {
        //Listing_Standard row = new Listing_Standard(GameFont.Small);
        //row.ColumnWidth = rect.width;
        //  row.Begin(rect);
        rect.yMin += 15f;
        rect.yMax -= 15f;

        var defaultColumnWidth = rect.width - 50;
        var row = new Listing_Standard { ColumnWidth = defaultColumnWidth };


        var outRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        var scrollRect = new Rect(0f, 0f, rect.width - 16f, rect.height * 2f);
        Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect);

        row.Begin(scrollRect);

        if (row.ButtonTextLabeled("fogAlphaSetting_title".Translate(), ("fogAlphaSetting_" + fogAlpha).Translate()))
        {
            var list = new List<FloatMenuOption>();
            foreach (var obj in Enum.GetValues(typeof(FogAlpha)))
            {
                var localValue3 = (FogAlpha)obj;
                list.Add(new FloatMenuOption(("fogAlphaSetting_" + localValue3).Translate(), delegate
                {
                    fogAlpha = localValue3;
                    ApplySettings();
                }));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }

        Text.Font = GameFont.Tiny;
        row.Label("fogAlphaSetting_desc".Translate());
        Text.Font = GameFont.Small;

        if (row.ButtonTextLabeled("fogFadeSpeedSetting_title".Translate(),
                ("fogFadeSpeedSetting_" + fogFadeSpeed).Translate()))
        {
            var list2 = new List<FloatMenuOption>();
            foreach (var obj2 in Enum.GetValues(typeof(FogFadeSpeedEnum)))
            {
                var localValue2 = (FogFadeSpeedEnum)obj2;
                list2.Add(new FloatMenuOption(("fogFadeSpeedSetting_" + localValue2).Translate(), delegate
                {
                    fogFadeSpeed = localValue2;
                    ApplySettings();
                }));
            }

            Find.WindowStack.Add(new FloatMenu(list2));
        }

        Text.Font = GameFont.Tiny;
        row.Label("fogFadeSpeedSetting_desc".Translate());
        Text.Font = GameFont.Small;
        AddGap(row);
        row.Label("baseViewRange".Translate() + ": " + baseViewRange.ToString(), -1f, "baseViewRange".Translate());
        baseViewRange = (int)row.Slider(baseViewRange, 10f, 100);
        row.Label("baseHearingRange".Translate() + ": " + Math.Round(baseHearingRange, 1).ToString());
        baseHearingRange = row.Slider(baseHearingRange, 0, 30);
        AddGap(row);
        row.Label("buildingVisionMod".Translate() + ": " + Math.Round(buildingVisionModifier, 2).ToString(), -1f,
            "buildingVisionModDesc".Translate());
        buildingVisionModifier = row.Slider(buildingVisionModifier, 0.2f, 2);
        row.Label("animalVisionModDesc".Translate() + ": " + Math.Round(animalVisionModifier, 2).ToString());
        animalVisionModifier = row.Slider(animalVisionModifier, 0.2f, 2);
        row.Label("turretVisionModDesc".Translate() + ": " + Math.Round(turretVisionModifier, 2).ToString());
        turretVisionModifier = row.Slider(turretVisionModifier, 0.2f, 2);
        AddGap(row);
        row.CheckboxLabeled("allyGiveVision".Translate(), ref allyGiveVision, "allyGiveVision".Translate());
        row.CheckboxLabeled("prisonerGiveVision".Translate(), ref prisonerGiveVision, "prisonerGiveVision".Translate());
        row.CheckboxLabeled("mapRevealAtStart".Translate(), ref mapRevealAtStart, "mapRevealAtStart".Translate());

        row.CheckboxLabeled("wildLifeTabVisible".Translate(), ref wildLifeTabVisible,
            "wildLifeTabVisibleDesc".Translate());
        row.CheckboxLabeled("NeedWatcher".Translate(), ref needWatcher, "NeedWatcherDesc".Translate());

        AddGap(row);
        row.CheckboxLabeled("hideEventNegative".Translate(), ref hideEventNegative, "hideEventNegative".Translate());
        row.CheckboxLabeled("hideEventNeutral".Translate(), ref hideEventNeutral, "hideEventNeutral".Translate());
        row.CheckboxLabeled("hideEventPositive".Translate(), ref hideEventPositive, "hideEventPositive".Translate());
        row.CheckboxLabeled("hideThreatBig".Translate(), ref hideThreatBig, "hideThreatBig".Translate());
        row.CheckboxLabeled("hideThreatSmall".Translate(), ref hideThreatSmall, "hideThreatSmall".Translate());
        AddGap(row);
        row.CheckboxLabeled("censorMode".Translate(), ref censorMode, "censorMode".Translate());
        row.CheckboxLabeled("hideSpeakBubble".Translate(), ref hideSpeakBubble, "hideSpeakBubbleDesc".Translate());
        row.CheckboxLabeled("aiSmart".Translate(), ref aiSmart, "aiSmartDesc".Translate());


        row.End();
        Widgets.EndScrollView();
    }

    public static void AddGap(Listing_Standard listing_Standard, float value = 12f)
    {
        listing_Standard.Gap(value);
        listing_Standard.GapLine(value);
    }

    public static void ApplySettings()
    {
        SectionLayer_FoVLayer.prefFadeSpeedMult = (int)fogFadeSpeed;
        SectionLayer_FoVLayer.prefEnableFade = fogFadeSpeed != FogFadeSpeedEnum.Disabled;
        SectionLayer_FoVLayer.prefFogAlpha = (byte)fogAlpha;
        if (Current.ProgramState != ProgramState.Playing)
        {
            return;
        }

        foreach (var map in Find.Maps)
        {
            map.mapDrawer?.RegenerateEverythingNow();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref fogFadeSpeed, "fogFadeSpeed", FogFadeSpeedEnum.Medium);
        Scribe_Values.Look(ref fogAlpha, "fogAlpha", FogAlpha.Medium);
        Scribe_Values.Look(ref baseViewRange, "baseViewRange", 60);
        Scribe_Values.Look(ref buildingVisionModifier, "buildingVisionMod", 1);
        Scribe_Values.Look(ref animalVisionModifier, "animalVisionMod", 0.5f);
        Scribe_Values.Look(ref turretVisionModifier, "turretVisionMod", 0.7f);
        Scribe_Values.Look(ref baseHearingRange, "baseHearingRange", 10);
        Scribe_Values.Look(ref wildLifeTabVisible, "wildLifeTabVisible", true);
        Scribe_Values.Look(ref prisonerGiveVision, "prisonerGiveVision");
        Scribe_Values.Look(ref mapRevealAtStart, "mapRevealAtStart");
        Scribe_Values.Look(ref allyGiveVision, "allyGiveVision");
        Scribe_Values.Look(ref needWatcher, "needWatcher", true);
        Scribe_Values.Look(ref needMemoryStorage, "needMemoryStorage", true);
        Scribe_Values.Look(ref hideEventNegative, "hideEventNegative");
        Scribe_Values.Look(ref hideEventNeutral, "hideEventNeutral");
        Scribe_Values.Look(ref hideEventPositive, "hideEventPositive");
        Scribe_Values.Look(ref hideThreatBig, "hideThreatBig");
        Scribe_Values.Look(ref hideThreatSmall, "hideThreatSmall");
        Scribe_Values.Look(ref censorMode, "censorMode");
        Scribe_Values.Look(ref hideSpeakBubble, "hideSpeakBubble");
        Scribe_Values.Look(ref aiSmart, "aiSmart");

        ApplySettings();
    }
}