using Verse;

namespace Powerless {
  
  internal class TurnipItemSpawner : ItemSpawner {

    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
      base.SpawnSetup(map, respawningAfterLoad);
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip"), 1, 2, map);
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip_Green"), 1, 4, map);
      Destroy();
    }
  }
}
