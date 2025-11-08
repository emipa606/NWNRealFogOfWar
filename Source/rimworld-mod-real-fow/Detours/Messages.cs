using HarmonyLib;
using RimWorld.Planet;
using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class Messages
{
    public static bool Message_Prefix(string text, ref LookTargets lookTargets)
    {
        var value = Traverse.Create(typeof(Verse.Messages)).Method("AcceptsMessage", text, lookTargets)
            .GetValue<bool>();
        if (!value)
        {
            return true;
        }

        var hasThing = lookTargets.PrimaryTarget.HasThing;
        if (!hasThing)
        {
            return true;
        }

        var thing = lookTargets.PrimaryTarget.Thing;
        if (thing.Faction is { IsPlayer: true })
        {
            return true;
        }

        if (thing.Spawned && RfowSettings.HideThreatBig && !thing.FowIsVisible())
        {
            return false;
        }

        lookTargets = new GlobalTargetInfo(thing.Position, thing.Map);

        return true;
    }
}