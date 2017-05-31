using RimWorld;
using Verse;
using Verse.Sound;

namespace Powerless {

  public class CompChandelier : ThingComp {

    private Map parentMap;


    public override void PostSpawnSetup(bool respawningAfterLoad) {
      base.PostSpawnSetup(respawningAfterLoad);

      parentMap = parent.Map;
    }


    public override void CompTickRare() {
      base.CompTickRare();

      // Minify this if the ceiling is missing
      int occCells = 0;
      int roofCells = 0;
      foreach (IntVec3 current in parent.OccupiedRect()) {
        occCells++;
        if (!parentMap.roofGrid.Roofed(current)) {
          roofCells++;
        }
      }
      if (((float)(occCells - roofCells) / occCells) < 0.5f) {
        Minify();
      }
    }


    public virtual void Minify() {
      MinifiedThing package = parent.MakeMinified();
      GenPlace.TryPlaceThing(package, parent.Position, parentMap, ThingPlaceMode.Near);
      SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, parentMap));
    }
  }
}
