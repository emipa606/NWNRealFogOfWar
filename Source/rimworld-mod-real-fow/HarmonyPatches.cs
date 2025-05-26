using System;
using HarmonyLib;
using RimWorld;
using RimWorldRealFoW.Utils;
using Verse;
using Verse.Sound;

namespace RimWorldRealFoW;

internal class HarmonyPatches
{
    //Pawn will not target thing that is hidden by the fog
    [HarmonyPrefix]
    public static bool CanSeePreFix(ref bool __result, Thing seer, Thing target, Func<IntVec3, bool> validator = null)
    {
        if (seer is not Pawn seerPawn || seer.Faction == null
                                      || !RFOWSettings.aiSmart || seer.Faction == Faction.OfPlayer
                                      || !seerPawn.RaceProps.Humanlike)
        {
            return true;
        }

        __result = seer.Map.getMapComponentSeenFog().isShown(seer.Faction, target.Position);

        return __result;
    }

    //For overlays
    [HarmonyPrefix]
    public static bool DrawOverlayPrefix(Thing t)
    {
        return t.fowIsVisible();
    }

    //For Silhouette
    [HarmonyPrefix]
    public static bool ShouldDrawSilhouettePrefix(Thing thing)
    {
        return thing.fowIsVisible();
    }

    ////For no dynamic sections
    //[HarmonyPrefix]
    //public static bool DrawDynamicSectionsPrefix(Section __instance)
    //{
    //    __instance.DrawSection();
    //    return false;
    //}


    //For interaction bubbles
    [HarmonyPrefix]
    public static bool DrawBubblePrefix(Pawn pawn, bool isSelected, float scale)
    {
        if (!RFOWSettings.hideSpeakBubble)
        {
            return true;
        }

        return pawn is not { IsColonist: false, Map: not null } ||
               pawn.Map.getMapComponentSeenFog().isShown(Faction.OfPlayer, pawn.Position);
    }

    //For suppressing letter
    [HarmonyPrefix]
    public static bool ReceiveLetterPrefix(ref Letter let)
    {
        if (let.def == LetterDefOf.NegativeEvent && RFOWSettings.hideEventNegative)
        {
            return false;
        }

        if (let.def == LetterDefOf.NeutralEvent && RFOWSettings.hideEventNeutral)
        {
            return false;
        }

        if (let.def == LetterDefOf.PositiveEvent && RFOWSettings.hideEventPositive)
        {
            return false;
        }

        if (let.def == LetterDefOf.ThreatBig && RFOWSettings.hideThreatBig)
        {
            return false;
        }

        return let.def != LetterDefOf.ThreatSmall || !RFOWSettings.hideThreatSmall;
    }
    // Registers sustainers in a dictionary to be later removed when Thing is hidden
    [HarmonyPatch(typeof(SustainerManager), nameof(SustainerManager.RegisterSustainer))]
    public static class Patch_RegisterSustainer
    {
        public static void Postfix(Sustainer __result)
        {
            if (__result?.info.Maker.Thing is Thing t)
                FoW_AudioCache.Register(t, __result);
        }
    }
    // Removes sustainers from the dictionary if the sustainer is ended on it's own
    [HarmonyPatch(typeof(Sustainer), nameof(Sustainer.End))]
    public static class Patch_Sustainer_End
    {
        public static void Postfix(Sustainer __instance)
        {
            FoW_AudioCache.Unregister(__instance);
        }
    }
}