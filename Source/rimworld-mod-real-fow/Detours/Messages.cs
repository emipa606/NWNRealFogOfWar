using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class Messages
{
    public static void Message_Prefix(string text, ref LookTargets lookTargets)
    {
        var value = Traverse.Create(typeof(Verse.Messages)).Method("AcceptsMessage", text, lookTargets)
            .GetValue<bool>();
        if (!value)
        {
            return;
        }

        var hasThing = lookTargets.PrimaryTarget.HasThing;
        if (!hasThing)
        {
            return;
        }

        var thing = lookTargets.PrimaryTarget.Thing;
        if (thing.Faction is not { IsPlayer: true })
        {
            lookTargets = new GlobalTargetInfo(thing.Position, thing.Map);
        }
    }
}