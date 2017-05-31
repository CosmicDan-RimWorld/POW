﻿using Verse;

namespace Powerless {

  public class PlaceWorker_Roofed : PlaceWorker {

    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {

      foreach (IntVec3 current in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size)) {
        if (!Map.roofGrid.Roofed(current)) {
          return new AcceptanceReport ("POW_NeedsRoof".Translate(new object[] { checkingDef.LabelCap }));
        }
      }

      return true;
    }
  }
}
