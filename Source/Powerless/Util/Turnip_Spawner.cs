using CorePanda;
using Verse;

namespace Powerless {
  
  public class Turnip_Spawner : ItemSpawner {

    public override void SpawnSetup() {
      base.SpawnSetup();
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip"), 1, 2);
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip_Green"), 1, 4);
      Destroy();
    }
  }
}
