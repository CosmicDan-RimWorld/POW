using Verse;

namespace Powerless {

  internal class PlaceWorker_Chandelier : PlaceWorker {

    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
      // Don't allow placing on big things
      foreach (IntVec3 c in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size)) {
        if (c.GetEdifice(Map) != null) {
          if (c.GetEdifice(Map).def.blockWind == true || c.GetEdifice(Map).def.holdsRoof == true) {
            return new AcceptanceReport("POW_ObjectTooTall".Translate(new object[] { c.GetEdifice(Map).LabelCap, checkingDef.LabelCap }));
          }
        }
        if (c.GetThingList(Map).Find(ch => ch.def.defName == "POW_GlowstoneChandelier") != null) {
          return new AcceptanceReport("IdenticalThingExists".Translate());
        }
      }

      // Otherwise, accept placing
      return true;
    }
  }
}
