using System;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace Powerless {

  public class WorkGiver_FillCoolingTowerDirect : WorkGiver_Scanner {

    public override ThingRequest PotentialWorkThingRequest {
      get {
        return ThingRequest.ForDef(ThingDef.Named("POW_CoolingTower"));
      }
    }

    public override PathEndMode PathEndMode {
      get {
        return PathEndMode.ClosestTouch;
      }
    }


    public override bool HasJobOnThing(Pawn pawn, Thing t) {
      if (t.Faction != pawn.Faction) {
        return false;
      }
      if (t.IsForbidden(pawn)) {
        return false;
      }
      if (pawn.Faction == Faction.OfPlayer && !Find.AreaHome[t.Position]) {
        return false;
      }
      Building_CoolingTower tower = t as Building_CoolingTower;
      if (tower == null) {
        return false;
      }
      if (!tower.CanAcceptBuckets) {
        return false;
      }
      if (tower.EmptySpace <= 0) {
        return false;
      }
      if (!pawn.CanReserve(tower, 1)) {
        return false;
      }
      if (Find.DesignationManager.DesignationOn(tower, DesignationDefOf.Deconstruct) != null) {
        return false;
      }

      return true;
    }


    public override Job JobOnThing(Pawn pawn, Thing t) {
      Thing bucket;
      Predicate<Thing> validator = (Thing b) => (!b.IsForbidden(pawn)) && pawn.CanReserve(b, 1);
      bucket = GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForDef(ThingDef.Named("CP_FreshWaterBucket")), PathEndMode.ClosestTouch, TraverseParms.For(pawn, pawn.NormalMaxDanger()), 99f, validator);

      if (bucket != null) {
        Building_CoolingTower tower = t as Building_CoolingTower;
        int numToCarry = Mathf.Min(bucket.def.stackLimit, tower.EmptySpace);
        return new Job(DefDatabase<JobDef>.GetNamed("POW_Job_FillCoolingTowerDirect"), t, bucket) {
          maxNumToCarry = numToCarry
        }; 
      }

      return null;
    }
  }
}
