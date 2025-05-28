using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Mlie;
using RimWorld;
using RimWorldRealFoW.Detours;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static RimWorldRealFoW.HarmonyPatches;

namespace RimWorldRealFoW;

[StaticConstructorOnStartup]
public class RealFoWModStarter : Mod
{
    private static readonly Harmony harmony;

    static RealFoWModStarter()
    {
        harmony = new Harmony("com.github.lukakama.rimworldmodrealfow");
        injectDetours();
        harmony = null;
    }

    public RealFoWModStarter(ModContentPack content) : base(content)
    {
        RFOWSettings.currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        LongEventHandler.QueueLongEvent(InjectComponents, "Real Fog of War - Init.", false, null);
        GetSettings<RFOWSettings>();
    }

    public override string SettingsCategory()
    {
        return Content.Name;
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        RFOWSettings.DoSettingsWindowContents(rect);
    }

    public static void InjectComponents()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
        {
            var category = thingDef.category;

            if (typeof(ThingWithComps).IsAssignableFrom(thingDef.thingClass)
                && (category == ThingCategory.Pawn
                    || category == ThingCategory.Building
                    || category == ThingCategory.Item
                    || category == ThingCategory.Filth
                    || category == ThingCategory.Gas
                    //|| category == ThingCategory.Projectile
                    || thingDef.IsBlueprint
                ))
            {
                AddComponentAsFirst(thingDef, CompMainComponent.COMP_DEF);
            }
        }
    }

    public static void AddComponentAsFirst(ThingDef def, CompProperties compProperties)
    {
        if (!def.comps.Contains(compProperties))
        {
            def.comps.Insert(0, compProperties);
        }
    }

    public static void injectDetours()
    {
        patchMethod(typeof(Verb), typeof(_Verb), "CanHitCellFromCellIgnoringRange");
        patchMethod(typeof(Selector), typeof(_Selector), nameof(Selector.Select));
        patchMethod(typeof(MouseoverReadout), typeof(_MouseoverReadout),
            nameof(MouseoverReadout.MouseoverReadoutOnGUI));
        patchMethod(typeof(BeautyUtility), typeof(_BeautyUtility), nameof(BeautyUtility.FillBeautyRelevantCells));
        patchMethod(typeof(MainTabWindow_Wildlife), typeof(_MainTabWindow_Wildlife), "get_Pawns");

        patchMethod(typeof(Pawn), typeof(_Pawn), nameof(Pawn.DrawGUIOverlay));
        patchMethod(typeof(GenMapUI), typeof(_GenMapUI), nameof(GenMapUI.DrawThingLabel), typeof(Thing), typeof(string),
            typeof(Color));

        patchMethod(typeof(SectionLayer_ThingsGeneral), typeof(_SectionLayer_ThingsGeneral), "TakePrintFrom");
        patchMethod(typeof(SectionLayer_ThingsPowerGrid), typeof(_SectionLayer_ThingsPowerGrid), "TakePrintFrom");
        patchMethod(typeof(ReservationUtility), typeof(_ReservationUtility), nameof(ReservationUtility.CanReserve));
        patchMethod(typeof(ReservationUtility), typeof(_ReservationUtility),
            nameof(ReservationUtility.CanReserveAndReach));
        patchMethod(typeof(HaulAIUtility), typeof(_HaulAIUtility), nameof(HaulAIUtility.HaulToStorageJob));
        patchMethod(typeof(EnvironmentStatsDrawer), typeof(_EnvironmentStatsDrawer), "ShouldShowWindowNow");

        patchMethod(typeof(Messages), typeof(_Messages), nameof(Messages.Message), typeof(string), typeof(LookTargets),
            typeof(MessageTypeDef), typeof(bool));
        patchMethod(typeof(LetterStack), typeof(_LetterStack), nameof(LetterStack.ReceiveLetter), typeof(TaggedString),
            typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest),
            typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool));

        patchMethod(typeof(MoteBubble), typeof(_MoteBubble), "DrawAt");
        patchMethod(typeof(GenView), typeof(_GenView), nameof(GenView.ShouldSpawnMotesAt), typeof(IntVec3), typeof(Map),
            typeof(bool));

        patchMethod(typeof(FertilityGrid), typeof(_FertilityGrid), "CellBoolDrawerGetBoolInt");
        patchMethod(typeof(TerrainGrid), typeof(_TerrainGrid), "CellBoolDrawerGetBoolInt");
        patchMethod(typeof(RoofGrid), typeof(_RoofGrid), nameof(RoofGrid.GetCellBool));

        //Area only designator
        patchMethod(typeof(Designator_AreaBuildRoof), typeof(_Designator_Prefix),
            nameof(Designator_AreaBuildRoof.CanDesignateCell));
        patchMethod(typeof(Designator_AreaNoRoof), typeof(_Designator_Prefix),
            nameof(Designator_AreaNoRoof.CanDesignateCell));
        patchMethod(typeof(Designator_ZoneAdd_Growing), typeof(_Designator_Prefix),
            nameof(Designator_ZoneAdd_Growing.CanDesignateCell));
        patchMethod(typeof(Designator_ZoneAddStockpile), typeof(_Designator_Prefix),
            nameof(Designator_ZoneAddStockpile.CanDesignateCell));

        //Area+Designator
        patchMethod(typeof(Designator_Claim), typeof(_Designator_Prefix), nameof(Designator_Claim.CanDesignateCell));
        patchMethod(typeof(Designator_Claim), typeof(_Designator_Prefix), nameof(Designator_Claim.CanDesignateThing));
        patchMethod(typeof(Designator_Deconstruct), typeof(_Designator_Prefix),
            nameof(Designator_Deconstruct.CanDesignateCell));
        patchMethod(typeof(Designator_Deconstruct), typeof(_Designator_Prefix),
            nameof(Designator_Deconstruct.CanDesignateThing));
        patchMethod(typeof(Designator_Haul), typeof(_Designator_Prefix), nameof(Designator_Haul.CanDesignateCell));
        patchMethod(typeof(Designator_Haul), typeof(_Designator_Prefix), nameof(Designator_Haul.CanDesignateThing));
        patchMethod(typeof(Designator_Hunt), typeof(_Designator_Prefix), nameof(Designator_Hunt.CanDesignateCell));
        patchMethod(typeof(Designator_Hunt), typeof(_Designator_Prefix), nameof(Designator_Hunt.CanDesignateThing));
        patchMethod(typeof(Designator_Plants), typeof(_Designator_Prefix), nameof(Designator_Plants.CanDesignateCell));
        patchMethod(typeof(Designator_Plants), typeof(_Designator_Prefix), nameof(Designator_Plants.CanDesignateThing));
        patchMethod(typeof(Designator_PlantsHarvest), typeof(_Designator_Prefix),
            nameof(Designator_PlantsHarvest.CanDesignateThing));
        patchMethod(typeof(Designator_PlantsHarvestWood), typeof(_Designator_Prefix),
            nameof(Designator_PlantsHarvestWood.CanDesignateThing));
        patchMethod(typeof(Designator_RemoveFloor), typeof(_Designator_Prefix),
            nameof(Designator_RemoveFloor.CanDesignateCell));
        patchMethod(typeof(Designator_SmoothSurface), typeof(_Designator_Prefix),
            nameof(Designator_SmoothSurface.CanDesignateCell));
        patchMethod(typeof(Designator_Tame), typeof(_Designator_Prefix), nameof(Designator_Tame.CanDesignateCell));
        patchMethod(typeof(Designator_Tame), typeof(_Designator_Prefix), nameof(Designator_Tame.CanDesignateThing));
        patchMethod(typeof(Designator_Uninstall), typeof(_Designator_Prefix),
            nameof(Designator_Uninstall.CanDesignateCell));

        //PLacing designator
        patchMethod(typeof(Designator_Uninstall), typeof(_Designator_Prefix),
            nameof(Designator_Uninstall.CanDesignateThing));
        patchMethod(typeof(Designator_Build), typeof(_Designator_Place_Postfix),
            nameof(Designator_Build.CanDesignateCell));
        patchMethod(typeof(Designator_Install), typeof(_Designator_Place_Postfix),
            nameof(Designator_Install.CanDesignateCell));

        //Specific designation
        patchMethod(typeof(Designator_Mine), typeof(_Designator_Mine), nameof(Designator_Mine.CanDesignateCell));

        //Designation
        patchMethod(typeof(Designation), typeof(_Designation), nameof(Designation.Notify_Added));
        patchMethod(typeof(Designation), typeof(_Designation), "Notify_Removing");

        // Sustainer cache
        patchMethod(typeof(SustainerManager), typeof(Patch_RegisterSustainer), nameof(SustainerManager.RegisterSustainer));
        patchMethod(typeof(Sustainer), typeof(Patch_UnregisterSustainer), nameof(Sustainer.End));

        /* Filth checks
        patchMethod(typeof(Thing), typeof(Patch_Filth_Draw), nameof(Filth.DrawNowAt));
        patchMethod(typeof(Filth), typeof(Patch_Filth_Destroy), nameof(Filth.Destroy));
        */

        harmony.Patch(
            typeof(AttackTargetFinder).GetMethod(nameof(AttackTargetFinder.CanSee)),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.CanSeePreFix))));

        Log.Message("Prefixed method AttackTargetFinder_CanSee.");
        harmony.Patch(
            typeof(LetterStack).GetMethod(nameof(LetterStack.ReceiveLetter), [
                typeof(Letter), typeof(string), typeof(int), typeof(bool)
            ]),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.ReceiveLetterPrefix))));

        Log.Message("Prefixed method LetterStack_ReceiveLetter.");

        harmony.Patch(
            typeof(OverlayDrawer).GetMethod(nameof(OverlayDrawer.DrawOverlay), [
                typeof(Thing), typeof(OverlayTypes)
            ]),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.DrawOverlayPrefix))));
        /*
        harmony.Patch(
            typeof(Filth).GetMethod(nameof(Filth.drawn), [
                typeof(Filth)
            ]),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.Patch_Filth_DrawAt))));

        harmony.Patch(
            typeof(Filth).GetMethod(nameof(Filth.Destroy), [
                typeof(Filth)
            ]),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.Patch_Filth_Destroy))));
        */
        Log.Message("Prefixed method OverlayDrawer_DrawOverlay.");

        harmony.Patch(
            typeof(SilhouetteUtility).GetMethod(nameof(SilhouetteUtility.ShouldDrawSilhouette), [
                typeof(Thing)
            ]),
            new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.ShouldDrawSilhouettePrefix))));

        Log.Message("Prefixed method SilhouetteUtility_ShouldDrawSilhouette.");

        harmony.Patch(
                    typeof(SoundStarter).GetMethod(
                    nameof(SoundStarter.PlayOneShot),
                    new[] { typeof(SoundDef), typeof(SoundInfo) }
            ),
                    prefix: new HarmonyMethod(
                    typeof(Patch_PlayOneShot).GetMethod(
                    nameof(Patch_PlayOneShot.Prefix)))
                );

        Log.Message("Prefixed method SoundStarter_PlayOneShot.");

        harmony.Patch(
            typeof(SoundStarter).GetMethod(
            nameof(SoundStarter.TrySpawnSustainer),
            new[] { typeof(SoundDef), typeof(SoundInfo) }
    ),
            prefix: new HarmonyMethod(
            typeof(Patch_TrySpawnSustainer).GetMethod(
            nameof(Patch_TrySpawnSustainer.Prefix))),
            postfix: new HarmonyMethod(
            typeof(Patch_TrySpawnSustainer).GetMethod(
            nameof(Patch_TrySpawnSustainer.Postfix)
        )
    )
);
        Log.Message("Prefixed method SoundStarter_TrySpawnSustainer.");



        

        //harmony.Patch(
        //    typeof(Section).GetMethod(nameof(Section.DrawDynamicSections), [
        //        typeof(CellRect)
        //    ]),
        //    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.DrawDynamicSectionsPrefix))));

        //Log.Message("Prefixed method Section_DrawDynamicSections.");

        if (!ModsConfig.IsActive("jaxe.bubbles"))
        {
            return;
        }

        var drawBubble = AccessTools.Method(
            "Bubbles.Interface.Bubbler:DrawBubble"
            //,new Type[] {typeof(Pawn), typeof(bool), typeof(float)}
        );
        if (drawBubble != null)
        {
            harmony.Patch(
                drawBubble,
                new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.DrawBubblePrefix)))
            );
            Log.Message("Interaction bubble is active. Patched");
        }
        else
        {
            Log.Warning("RFow is active but can't patch DrawBubble method");
        }
    }

    public static void patchMethod(Type sourceType, Type targetType, string methodName)
    {
        patchMethod(sourceType, targetType, methodName, null);
    }


    public static void patchMethod(Type sourceType, Type targetType, string methodName, params Type[] types)
    {
        MethodInfo method;
        if (types != null)
        {
            method = sourceType.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types,
                null);
        }
        else
        {
            method = sourceType.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        if (sourceType != method?.DeclaringType)
        {
            Log.Message(
                $"Inconsistent method declaring type for method {methodName}: expected {sourceType} but found {method?.DeclaringType}");
        }

        if (method != null)
        {
            MethodInfo methodInfo = null;
            if (types != null)
            {
                methodInfo = targetType.GetMethod($"{methodName}_Prefix",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null,
                    types, null);
                if (methodInfo == null)
                {
                    methodInfo = targetType.GetMethod($"{methodName}_Prefix",
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new[]
                        {
                            sourceType
                        }.Concat(types).ToArray(), null);
                }
            }

            if (methodInfo == null)
            {
                methodInfo = targetType.GetMethod($"{methodName}_Prefix",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            MethodInfo methodInfo2 = null;
            if (types != null)
            {
                methodInfo2 = targetType.GetMethod($"{methodName}_Postfix",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null,
                    types, null);
                if (methodInfo2 == null)
                {
                    methodInfo2 = targetType.GetMethod($"{methodName}_Postfix",
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new[]
                        {
                            sourceType
                        }.Concat(types).ToArray(), null);
                }
            }

            if (methodInfo2 == null)
            {
                methodInfo2 = targetType.GetMethod($"{methodName}_Postfix",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (methodInfo != null || methodInfo2 != null)
            {
                if (patchWithHarmony(method, methodInfo, methodInfo2))
                {
                    Log.Message($"Patched method {method} from source {sourceType} to {targetType}.");
                }
                else
                {
                    Log.Warning($"Unable to patch method {method} from source {sourceType} to {targetType}.");
                }
            }
            else
            {
                Log.Warning(
                    $"Target method prefix or suffix {methodName} not found for patch from source {sourceType} to {targetType}.");
            }
        }
        else
        {
            Log.Warning($"Source method {methodName} not found for patch from source {sourceType} to {targetType}.");
        }
    }

    public static bool patchWithHarmony(MethodInfo original, MethodInfo prefix, MethodInfo postfix)
    {
        bool result;
        try
        {
            var prefix2 = prefix != null ? new HarmonyMethod(prefix) : null;
            var postfix2 = postfix != null ? new HarmonyMethod(postfix) : null;
            harmony.Patch(original, prefix2, postfix2);
            result = true;
        }
        catch (Exception ex)
        {
            Log.Warning($"Error patching with Harmony: {ex.Message}");
            Log.Warning(ex.StackTrace);
            result = false;
        }

        return result;
    }
}