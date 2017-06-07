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
      foreach (IntVec3 c in parent.OccupiedRect()) {
        occCells++;
        if (!parentMap.roofGrid.Roofed(c)) {
          roofCells++;
        }
        if (c.GetEdifice(parent.Map).def.blockWind == true || c.GetEdifice(parent.Map).def.holdsRoof == true) {
          Minify();
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
