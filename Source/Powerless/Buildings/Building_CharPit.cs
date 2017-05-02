using Verse;

namespace Powerless {

  public class Building_CharPit : Building {

    private int burnTicks = 80;      // Burning Ticks left. Starts at 80 (8 hours, TickRare)
    public int BurnTicks { get { return burnTicks; } }
    Thing charcoalPit;


    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref burnTicks, "burnTicksC", 80);
    }


    public override void TickRare() {
      base.TickRare();

      // Make sure there is still a pit in this location
      charcoalPit = Position.GetFirstThing(Map, ThingDef.Named("POW_CharcoalPit"));
      //charcoalPit = Position.GetThingList(Map).Find(pit => pit is Building_CharcoalPit) as Building_CharcoalPit;

      // Deconstruct the charcoal, the pit has been removed
      if (Spawned && charcoalPit == null) {
        Destroy(DestroyMode.Deconstruct);
      }

      // decrease burnTick counter
      if (Spawned && burnTicks > 0) {
        burnTicks--;
      }

      // Spawn charcoal
      if (Spawned && burnTicks <= 0) {
        PlaceProduct();
      }
    }


    private void PlaceProduct() {
      Thing placedProduct = ThingMaker.MakeThing(ThingDef.Named("POW_Charcoal"));
      placedProduct.stackCount = 75;
      Destroy();
      GenPlace.TryPlaceThing(placedProduct, Position, Map, ThingPlaceMode.Direct);
    }
  }
}
