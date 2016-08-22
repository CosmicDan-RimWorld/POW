using System.Collections.Generic;

using CorePanda;
using RimWorld;
using Verse;
using Verse.Noise;

namespace Powerless {

  public class Building_ColdstoneBattery : Building {

    // Comps and variables
    private CompColdPusher coldpusherComp;
    private CompFlickable flickableComp;
    private CompRefuelable refuelableComp;
    private ModuleBase snowNoise;
    private static List<Room> touchingRooms = new List<Room>();
    private float snowRadius;


    public override void SpawnSetup() {
      base.SpawnSetup();
      // Register comps
      coldpusherComp = GetComp<CompColdPusher>();
      flickableComp = GetComp<CompFlickable>();
      refuelableComp = GetComp<CompRefuelable>();
    }


    public override void Tick() {
      // If the battery isn't turned off, and the room is hotter than it should be, and it has fuel, set CompColdPusher to be active this tick
      if ((flickableComp != null && flickableComp.SwitchIsOn) && Position.GetTemperature() > coldpusherComp.Props.coldPushMinTemperature && refuelableComp.HasFuel) {
        coldpusherComp.SimplePush();
        refuelableComp.CompTick();
        return;
      }
      // Add snow
      if (refuelableComp.HasFuel && ((Find.TickManager.TicksGame + this.HashOffset()) % 500 == 0)) {
        ExpandSnow();
      }
    }


    // Expand snow around the battery
    private void ExpandSnow() {
      if (this.snowNoise == null) {
        this.snowNoise = new Perlin(0.055, 2.0, 0.5, 3, Rand.Range(0, 651431), QualityMode.Low);
      }
      if (this.snowRadius < 1f) {
        this.snowRadius += 1.1f;
      }
      else {
        this.snowRadius += 0.2f;
      }
      if (this.snowRadius > 5f) {
        this.snowRadius = 5f;
      }

      touchingRooms.Clear();
      CellRect.CellRectIterator iterator = this.OccupiedRect().ExpandedBy(1).ClipInsideMap().GetIterator();
      while (!iterator.Done()) {
        Room room = iterator.Current.GetRoom();
        if (room != null && !touchingRooms.Contains(room)) {
          touchingRooms.Add(room);
        }
        iterator.MoveNext();
      }
      int num = GenRadial.NumCellsInRadius(snowRadius);
      for (int i = 0; i < num; i++) {
        IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
        if (intVec.InBounds()) {
          Room room2 = intVec.GetRoom();
          if (room2 != null) {
            int j = 0;
            while (j < touchingRooms.Count) {
              if (touchingRooms[j] == room2) {
                float num2 = snowNoise.GetValue(intVec);
                num2 += 1f;
                num2 *= 0.5f;
                if (num2 < 0.1f) {
                  num2 = 0.1f;
                }
                if (Find.SnowGrid.GetDepth(intVec) > 0.6f) {
                  break;
                }
                float lengthHorizontal = (intVec - base.Position).LengthHorizontal;
                float num3 = 1f - lengthHorizontal / this.snowRadius;
                Find.SnowGrid.AddDepth(intVec, num3 * 0.12f * num2);
                break;
              }
              else {
                j++;
              }
            }
          }
        }
      }
    }


    // Handle loading
    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref snowRadius, "snowRadius", 0f, false);
    }
  }
}
