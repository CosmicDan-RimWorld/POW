using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Verse;

namespace Powerless {

  public class PlaceWorker_CoolingTower : PlaceWorker {
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot) {
      IntVec3 intVec = center + IntVec3.South.RotatedBy(rot);
      GenDraw.DrawFieldEdges(new List<IntVec3>
      {
        center
      }, Color.white);
      GenDraw.DrawFieldEdges(new List<IntVec3>
      {
        intVec
      }, GenTemperature.ColorSpotCold);
      Room room = intVec.GetRoom();
      if (room != null) {
        if (!room.UsesOutdoorTemperature) {
          GenDraw.DrawFieldEdges(room.Cells.ToList<IntVec3>(), GenTemperature.ColorRoomCold);
        }
      }
    }
    public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot) {
      IntVec3 c = center + IntVec3.South.RotatedBy(rot);
      if (c.Impassable()) {
        return "POW_MustPlaceTowerWithFreeSpace".Translate();
      }
      return true;
    }
  }
}
