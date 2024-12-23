using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RimWorldRealFoW;

internal class JobDriver_SurveilCameraConsole : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var targetA = job.targetA;
        return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        this.FailOn(() => !((Building_CameraConsole)job.targetA.Thing).NeedWatcher());

        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

        var work = new Toil();

        work.tickAction = delegate
        {
            var actor = work.GetActor();
            var building_CameraConsole = job.targetA.Thing as Building_CameraConsole;
            building_CameraConsole?.Used();
            actor.GainComfortFromCellIfPossible(true);
        };
        work.defaultCompleteMode = ToilCompleteMode.Never;
        work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        work.activeSkill = () => SkillDefOf.Intellectual;
        yield return work;
    }
}