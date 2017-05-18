﻿using RimWorld;
using Verse;
using Verse.AI;

namespace Powerless {

  public class WorkGiver_TakeVinegarOutOfVinegarBarrel : WorkGiver_Scanner {

    public override ThingRequest PotentialWorkThingRequest {
      get {
        return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
      }
    }

    public override PathEndMode PathEndMode {
      get {
        return PathEndMode.Touch;
      }
    }


    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) {
      Building_VinegarBarrel vinegarBarrel = t as Building_VinegarBarrel;
      return vinegarBarrel != null && vinegarBarrel.Fermented && !t.IsBurning() && !t.IsForbidden(pawn) && pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced);
    }


    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) {
      return new Job(DefDatabase<JobDef>.GetNamed("POW_TakeVinegarOutOfVinegarBarrel"), t);
    }
  }
}
