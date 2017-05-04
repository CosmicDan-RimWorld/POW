using Verse;

namespace Powerless {
  
  internal class Turnip_Spawner : ThingWithComps {

    public override void SpawnSetup(Map map) {
      base.SpawnSetup(map);
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip"), 1, 2, map);
      SpawnRandomQuantity(ThingDef.Named("POW_Turnip_Green"), 1, 4, map);
      Destroy();
    }


    private void SpawnRandomQuantity(ThingDef tDef, int minToSpawn, int maxToSpawn, Map map) {
      int stack = Rand.RangeInclusive(minToSpawn, maxToSpawn);
      Thing placedProduct = ThingMaker.MakeThing(tDef);
      placedProduct.stackCount = stack;
      GenPlace.TryPlaceThing(placedProduct, Position, map, ThingPlaceMode.Near);
    }
  }
}
