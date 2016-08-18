using System.Collections.Generic;

using Verse.AI;

namespace Powerless {

  public class JobDriver_FillCoolingTowerDirect : JobDriver {

    private const TargetIndex TowerIndex = TargetIndex.A;
    private const TargetIndex BucketIndex = TargetIndex.B;


    protected override IEnumerable<Toil> MakeNewToils() {
      Building_CoolingTower Tower = TargetA.Thing as Building_CoolingTower;

      this.FailOnDestroyedNullOrForbidden(TowerIndex);
      this.FailOnDestroyedNullOrForbidden(BucketIndex);

      //Reserve resources
      yield return Toils_Reserve.Reserve(BucketIndex);
      yield return Toils_Reserve.ReserveQueue(BucketIndex);

      //Reserve tower
      yield return Toils_Reserve.Reserve(TowerIndex);

      Toil getToMaterialTarget = Toils_Goto.GotoThing(BucketIndex, PathEndMode.ClosestTouch)
        .FailOnSomeonePhysicallyInteracting(BucketIndex);
      yield return getToMaterialTarget;

      yield return Toils_Haul.JumpIfAlsoCollectingNextTargetInQueue(getToMaterialTarget, BucketIndex);

      yield return Toils_Haul.StartCarryThing(BucketIndex);

      Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TowerIndex);
      yield return carryToCell;

      // Wait before using water
      yield return Toils_General.Wait(750).WithProgressBarToilDelay(TowerIndex);

      // Use water
      Toil fill = new Toil();
      fill.initAction = () => {
        Tower.Notify_FilledWithBucket(fill.actor.carrier.CarriedThing.stackCount);
        fill.actor.carrier.CarriedThing.Destroy();
        pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
      };
      fill.defaultCompleteMode = ToilCompleteMode.Instant;
      yield return fill;

      // Release the reservation on the tower
      yield return Toils_Reserve.Release(TowerIndex);
    }
  }
}
