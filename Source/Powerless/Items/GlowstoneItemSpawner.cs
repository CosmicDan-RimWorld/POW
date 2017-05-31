using Verse;

namespace Powerless {

  internal class GlowstoneItemSpawner : ItemSpawner {

    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
      base.SpawnSetup(map, respawningAfterLoad);
      SpawnRandomQuantity(ThingDef.Named("POW_LargeGlowstone"), -2, 2, map);
      SpawnRandomQuantity(ThingDef.Named("POW_SmallGlowstone"), 1, 3, map);
      SpawnRandomQuantity(ThingDef.Named("POW_GlowstoneDust"), -5, 5, map);
      Destroy();
    }
  }
}
