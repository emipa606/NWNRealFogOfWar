using Verse;

namespace RimWorldRealFoW;

public class Verb_Look : Verb
{
    protected override bool TryCastShot()
    {
        bool result;
        if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map || currentTarget.Thing is not Pawn)
        {
            result = false;
        }
        else
        {
            if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out _))
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }

        return result;
    }
}