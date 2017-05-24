using RimWorld;
using Verse;

namespace Powerless {

  public class PlaceWorker_WindowInWall : PlaceWorker {

    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
      IntVec3 adjRelativeNorth = loc + IntVec3.North.RotatedBy(rot);
      IntVec3 adjRelativeSouth = loc + IntVec3.South.RotatedBy(rot);

      // Don't place out of bounds
      if (!loc.InBounds(Map)) {
        return false;
      }

      // Ensure there is enough room to spawn a glower as well
      for (int i = 0; i < 4; i++) {
        IntVec3 c = loc + GenAdj.CardinalDirections[i];
        if (!c.InBounds(Map)) {
          return false;
        }
      }

      Building edifice = loc.GetEdifice(Map);
      // Only allow placing on a constructed wall
      // Additional checks provided to hopefully catch any modded walls as well
      if (edifice == null || (edifice.def != ThingDefOf.Wall && (edifice.Faction != Faction.OfPlayer || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
        return "POW_WindowNeedsWall".Translate();
      }

      if (adjRelativeNorth.Impassable(Map) || adjRelativeSouth.Impassable(Map)) {
        return "POW_WindowImpassable".Translate();
      }

      return true;
    }
  }
}
