using System;
using System.Security.Cryptography;
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

    public static class Patch_RegisterSustainer
    {
        [HarmonyPostfix]
        public static void Postfix(Sustainer __result)
        {
            if (__result?.info.Maker.Thing is Thing t)
                FoW_AudioCache.Register(t, __result);
        }
    }
    public static class Patch_UnregisterSustainer
    {
        [HarmonyPostfix]
        public static void UnregisterSustainer(Sustainer __instance)
        {
            FoW_AudioCache.Unregister(__instance);
        }
    }

    /*
    public static class Patch_Filth_Draw
    {
        public static bool Prefix(Thing __instance)
        {
            if (__instance is Filth filth && RFOWSettings.doFilthReveal)
            {
                var map = filth.Map;
                var comp = map.getMapComponentSeenFog();
                int idx = map.cellIndices.CellToIndex(filth.Position);
                if (!comp.knownFilthCells[idx])
                    return false;  // skip drawing this filth
            }
            return true;  // draw everything else, and drawn filth once revealed
        }
    }

    public static class Patch_Filth_Destroy
    {
        [HarmonyPostfix]
        public static void Postfix(Filth __instance)
        {
            if (!RFOWSettings.doFilthReveal) return;

            var map = __instance.Map;
            int idx = map.cellIndices.CellToIndex(__instance.Position);
            var comp = map.getMapComponentSeenFog();

            // if no other filth remains at that cell, hide future new filth
            bool anyLeft = map.listerThings
                              .ThingsInGroup(ThingRequestGroup.Filth)
                              .Any(t => t.Position == __instance.Position);
            if (!anyLeft)
                comp.knownFilthCells[idx] = false;
        }
    }
    */

    public static class Patch_PlayOneShot
    {
        [HarmonyPrefix]
        public static bool Prefix(SoundDef soundDef, SoundInfo info)
        {
            Log.Message("One shot start");
            if (RFOWSettings.doAudioCheck && info.Maker.Map != null && info.Maker.Cell.InBounds(info.Maker.Map))
            {
                float f = FoW_AudioCache.GetAudibilityFactor(info.Maker, RFOWSettings.audioSourceRange);
                Log.Message($"[FoW] PlayOneShot fired for {soundDef.defName} at {info.Maker.Cell}");
                Log.Message($"[FoW] doAudioCheck = {RFOWSettings.doAudioCheck}");
                Log.Message($"[FoW] audibilityFactor = {f:0.00}");
                if (f <= 0f) return false;          // skip the original call entirely
                info.volumeFactor *= f;            // otherwise muffle via SoundInfo.volumeFactor
            }
            return true; // run the original PlayOneShot
        }
    }
    
    public static class Patch_TrySpawnSustainer
    {
        [HarmonyPrefix]
        public static bool Prefix(SoundDef soundDef, SoundInfo info)
        {
            if (RFOWSettings.doAudioCheck && info.Maker.Thing is Thing t)
            {
                float f = FoW_AudioCache.GetAudibilityFactor(t, RFOWSettings.audioSourceRange);
                Log.Message($"[FoW] TryPlaySustain fired for {info.Maker}");
                Log.Message($"doAudioCheck = {RFOWSettings.doAudioCheck}");
                Log.Message($"[FoW] audibilityFactor = {f:0.00}");
                if (f <= 0f) return false;    // mute entirely
                info.volumeFactor *= f;       // muffle looping sound

            }
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(Sustainer __result)
        {
            if (__result != null && __result.info.volumeFactor <= 0f)
                __result.End();
        }
    }


}