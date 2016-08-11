using System.Collections.Generic;

using RimWorld;
using Verse.AI;
using Verse;
using UnityEngine;

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
      yield return Toils_Reserve.ReserveQueue(TowerIndex);

      Toil getToMaterialTarget = Toils_Goto.GotoThing(BucketIndex, PathEndMode.ClosestTouch)
        .FailOnSomeonePhysicallyInteracting(BucketIndex);
      yield return getToMaterialTarget;

      yield return Toils_Haul.StartCarryThing(BucketIndex);

      yield return Toils_Haul.JumpIfAlsoCollectingNextTargetInQueue(getToMaterialTarget, BucketIndex);

      Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TowerIndex);
      yield return carryToCell;

      // Wait before using water
      yield return Toils_General.Wait(750).WithProgressBarToilDelay(TowerIndex);

      // Use water
      Toil fill = new Toil();
      fill.initAction = () => {
        fill.actor.carrier.CarriedThing.Destroy();
        Tower.Notify_FilledWithBucket();
        pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
      };
      fill.defaultCompleteMode = ToilCompleteMode.Instant;
      yield return fill;

      // Release the reservation on the tower
      yield return Toils_Reserve.Release(TowerIndex);
    }
  }
}
