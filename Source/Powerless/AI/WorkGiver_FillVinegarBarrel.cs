﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace Powerless {

  public class WorkGiver_FillVinegarBarrel : WorkGiver_Scanner {


    private static string TemperatureTrans;
    private static string NoIngredientTrans;

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


    public static void Reset() {
      TemperatureTrans = "BadTemperature".Translate().ToLower();
      NoIngredientTrans = "POW_NoIngredient".Translate();
    }


    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) {
      Building_VinegarBarrel vinegarBarrel = t as Building_VinegarBarrel;
      if (vinegarBarrel == null || vinegarBarrel.Fermented || vinegarBarrel.SpaceLeftForJuice <= 0) {
        return false;
      }
      float ambientTemperature = vinegarBarrel.AmbientTemperature;
      CompProperties_TemperatureRuinable compProperties = vinegarBarrel.def.GetCompProperties<CompProperties_TemperatureRuinable>();
      if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f) {
        JobFailReason.Is(TemperatureTrans);
        return false;
      }
      if (t.IsForbidden(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced)) {
        return false;
      }
      if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null) {
        return false;
      }
      if (FindIngredient(pawn, vinegarBarrel) == null) {
        JobFailReason.Is(NoIngredientTrans);
        return false;
      }
      return !t.IsBurning();
    }


    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) {
      Building_VinegarBarrel building_AlcoholBarrel = (Building_VinegarBarrel)t;
      Thing t2 = FindIngredient(pawn, building_AlcoholBarrel);
      return new Job(DefDatabase<JobDef>.GetNamed("POW_FillVinegarBarrel"), t, t2) {
        count = building_AlcoholBarrel.SpaceLeftForJuice
      };
    }


    private Thing FindIngredient(Pawn pawn, Building_VinegarBarrel barrel) {
      Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x);
      Predicate<Thing> validator = predicate;
      return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDef.Named("POW_VinegarJuice")), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator);
    }
  }
}
