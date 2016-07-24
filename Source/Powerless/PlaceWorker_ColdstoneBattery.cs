using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Verse;

namespace Powerless {

  public class PlaceWorker_ColdstoneBattery : PlaceWorker {

    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot) {
      GenDraw.DrawFieldEdges(new List<IntVec3>
      {
        center
      }, Color.white);
      Room room = center.GetRoom();
      if (room != null) {
        if (!room.UsesOutdoorTemperature) {
          GenDraw.DrawFieldEdges(room.Cells.ToList<IntVec3>(), GenTemperature.ColorRoomCold);
        }
      }
    }
  }
}
