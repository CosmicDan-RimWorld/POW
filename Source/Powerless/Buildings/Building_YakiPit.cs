using Verse;

namespace Powerless {

  public class Building_YakiPit : Building {

    private int burnTicks = 30;         // Burning Ticks left. Starts at 30 (3 hours, TickRare)
    public int BurnTicks { get { return burnTicks; } }
    Building_CharcoalPit charcoalPit;


    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref burnTicks, "burnTicksY", 30);
    }


    public override void TickRare() {
      base.TickRare();

      charcoalPit = Position.GetThingList().Find(pit => pit is Building_CharcoalPit) as Building_CharcoalPit;

      if (Spawned && charcoalPit == null) {
        Destroy(DestroyMode.Deconstruct);
      }

      if (Spawned && burnTicks > 0) {
        burnTicks--;
      }

      if (Spawned && burnTicks <= 0) {
        PlaceProduct();
      }
    }


    private void PlaceProduct() {
      Thing placedProduct = ThingMaker.MakeThing(ThingDef.Named("POW_Yakisugi"));
      placedProduct.stackCount = 25;
      Destroy();
      GenPlace.TryPlaceThing(placedProduct, Position, ThingPlaceMode.Direct);
    }
  }
}
