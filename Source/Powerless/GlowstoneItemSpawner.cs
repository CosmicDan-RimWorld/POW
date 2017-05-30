using Verse;

namespace Powerless {

  internal class GlowstoneItemSpawner : Thing {


    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
      base.SpawnSetup(map, respawningAfterLoad);
      SpawnRandomQuantity(ThingDef.Named("POW_LargeGlowstone"), -4, 2, map);
      SpawnRandomQuantity(ThingDef.Named("POW_SmallGlowstone"), 1, 8, map);
      SpawnRandomQuantity(ThingDef.Named("POW_GlowstoneDust"), -5, 10, map);
      Destroy();
    }


    private void SpawnRandomQuantity(ThingDef tDef, int minToSpawn, int maxToSpawn, Map map) {
      int stack = Rand.RangeInclusive(minToSpawn, maxToSpawn);
      if (stack <= 0) {
        return;
      }
      Thing placedProduct = ThingMaker.MakeThing(tDef);
      placedProduct.stackCount = stack;
      GenPlace.TryPlaceThing(placedProduct, Position, map, ThingPlaceMode.Near);
    }
  }
}
