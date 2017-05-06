using System.Text;

using RimWorld;
using Verse;

namespace Powerless {

  internal class Building_VinegarBarrel : Building {

    // Ticks left for fermenting to finish
    private int fermentTicks = 0;

    // Ticks to reach before spawning a product
    // 2400 == 10 days of rare ticks
    private int targetTicks = 2400;


    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.Look(ref fermentTicks, "POW_fermentTicks", 0);
    }


    public override void TickRare() {
      base.TickRare();

      // Increment the fermenting progress
      if (fermentTicks < targetTicks) {
        fermentTicks++;
      }

      // Produce vinegar when finished
      if (fermentTicks >= targetTicks) {
        PlaceProduct();
      }
    }


    private void PlaceProduct() {
      Destroy();

      // Create 20 bottles of vinegar
      Thing vinegar = ThingMaker.MakeThing(ThingDef.Named("POW_Vinegar"));
      vinegar.stackCount = 20;

      // Return the wood used to craft this barrel
      Thing wood = ThingMaker.MakeThing(ThingDefOf.WoodLog);
      wood.stackCount = 25;

      GenPlace.TryPlaceThing(vinegar, Position, Map, ThingPlaceMode.Near);
      GenPlace.TryPlaceThing(wood, Position, Map, ThingPlaceMode.Near);
    }


    public override string GetInspectString() {
      StringBuilder stringBuilder = new StringBuilder();
      // Get inherited string data
      stringBuilder.Append(base.GetInspectString());

      // Display the fermenting progress
      stringBuilder.AppendLine("POW_Progress".Translate() + ": (" + (((float)fermentTicks / targetTicks) * 100).ToString("#0.00") + "%)");

      return stringBuilder.ToString();
    }
  }
}
