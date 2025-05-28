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
    public static bool CanSeePreFix(ref bool __result, Thing seer, Thing target)
    {
        if (seer is not Pawn seerPawn || seer.Faction == null
                                      || !RfowSettings.AISmart || seer.Faction == Faction.OfPlayer
                                      || !seerPawn.RaceProps.Humanlike)
        {
            return true;
        }

        __result = seer.Map.GetMapComponentSeenFog().IsShown(seer.Faction, target.Position);

        return __result;
    }

    //For overlays
    [HarmonyPrefix]
    public static bool DrawOverlayPrefix(Thing t)
    {
        return t.FowIsVisible();
    }

    //For Silhouette
    [HarmonyPrefix]
    public static bool ShouldDrawSilhouettePrefix(Thing thing)
    {
        return thing.FowIsVisible();
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
    public static bool DrawBubblePrefix(Pawn pawn)
    {
        if (!RfowSettings.HideSpeakBubble)
        {
            return true;
        }

        return pawn is not { IsColonist: false, Map: not null } ||
               pawn.Map.GetMapComponentSeenFog().IsShown(Faction.OfPlayer, pawn.Position);
    }

    //For suppressing letter
    [HarmonyPrefix]
    public static bool ReceiveLetterPrefix(ref Letter let)
    {
        if (let.def == LetterDefOf.NegativeEvent && RfowSettings.HideEventNegative)
        {
            return false;
        }

        if (let.def == LetterDefOf.NeutralEvent && RfowSettings.HideEventNeutral)
        {
            return false;
        }

        if (let.def == LetterDefOf.PositiveEvent && RfowSettings.HideEventPositive)
        {
            return false;
        }

        if (let.def == LetterDefOf.ThreatBig && RfowSettings.HideThreatBig)
        {
            return false;
        }

        return let.def != LetterDefOf.ThreatSmall || !RfowSettings.HideThreatSmall;
    }
    // Registers sustainers in a dictionary to be later removed when Thing is hidden

    public static class Patch_RegisterSustainer
    {
        [HarmonyPostfix]
        public static void Postfix(Sustainer newSustainer)
        {
            if (newSustainer?.info.Maker.Thing is { } thing)
            {
                FoW_AudioCache.Register(thing, newSustainer);
            }
        }
    }

    public static class Patch_UnregisterSustainer
    {
        [HarmonyPostfix]
        public static void Postfix(Sustainer __instance)
        {
            FoW_AudioCache.Unregister(__instance);
        }
    }

    public static class Patch_PlayOneShot
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SoundInfo info)
        {
            if (!RfowSettings.DoAudioCheck || info.Maker.Map == null || !info.Maker.Cell.InBounds(info.Maker.Map))
            {
                return true; // run the original PlayOneShot
            }

            var audibilityFactor = FoW_AudioCache.GetAudibilityFactor(info.Maker, RfowSettings.AudioSourceRange);
            if (audibilityFactor <= 0f)
            {
                return false; // skip the original call entirely
            }

            info.volumeFactor *= audibilityFactor; // otherwise muffle via SoundInfo.volumeFactor

            return true; // run the original PlayOneShot
        }
    }

    public static class Patch_TrySpawnSustainer
    {
        [HarmonyPrefix]
        public static bool Prefix(SoundInfo info)
        {
            if (!RfowSettings.DoAudioCheck || info.Maker.Thing is not { } thing)
            {
                return true;
            }

            var audibilityFactor = FoW_AudioCache.GetAudibilityFactor(thing, RfowSettings.AudioSourceRange);
            if (audibilityFactor <= 0f)
            {
                return false; // mute entirely
            }

            info.volumeFactor *= audibilityFactor; // muffle looping sound

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Sustainer __result)
        {
            if (__result is { info.volumeFactor: <= 0f })
            {
                __result.End();
            }
        }
    }
}