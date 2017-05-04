using System.Text;

using Verse;

namespace Powerless {

  internal class Building_CharcoalPit : Building {

    // Burning Ticks left. Starts at 80 (8 hours, TickRare)
    private int burnTicks = 80;


    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref burnTicks, "POW_burnTicks", 80);
    }


    public override void TickRare() {
      base.TickRare();

      // decrease burnTick counter
      if (burnTicks > 0) {
        burnTicks--;
      }

      // Spawn charcoal
      if (burnTicks <= 0) {
        PlaceProduct(Map);
      }
    }


    private void PlaceProduct(Map map) {
      Thing placedProduct = ThingMaker.MakeThing(ThingDef.Named("POW_Charcoal"));
      placedProduct.stackCount = 75;
      Destroy();
      GenPlace.TryPlaceThing(placedProduct, Position, map, ThingPlaceMode.Direct);
    }


    public override string GetInspectString() {
      StringBuilder stringBuilder = new StringBuilder();

      // Display the burning progress
      stringBuilder.AppendLine(ThingDef.Named("POW_Charcoal").LabelCap + " " + "POW_Progress".Translate().ToLower() + ": (" + (100f - (burnTicks / 0.8f)).ToString("#0.00") + "%)");

      return stringBuilder.ToString();
    }
  }
}
