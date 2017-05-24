using System.Collections.Generic;

using RimWorld;
using Verse;

namespace Powerless {

  internal class Building_Window : Building {

    private readonly WindowGlow bright = new WindowGlow(new ColorInt(175, 175, 165, 0), 9f, true);
    private readonly WindowGlow lit_5 =  new WindowGlow(new ColorInt(150, 150, 140, 0), 8f);
    private readonly WindowGlow lit_4 =  new WindowGlow(new ColorInt(125, 125, 120, 0), 7f);
    private readonly WindowGlow lit_3 =  new WindowGlow(new ColorInt(100, 100, 95,  0), 6f);
    private readonly WindowGlow lit_2 =  new WindowGlow(new ColorInt(67,  67,  75,  0), 4f);
    private readonly WindowGlow lit_1 =  new WindowGlow(new ColorInt(30,  26,  37,  0), 2f);
    private readonly WindowGlow dark =   new WindowGlow(new ColorInt(1,   1,   2,   0), 1f);

    private CellRect cellRect1;
    private CellRect cellRect2;
    private CellRect glowerCellRect;
    private IntVec3 viewCell1;
    private IntVec3 viewCell2;
    private Building glower;
    private IntVec3 viewCell;

    private float cachedSunlight;
    private WindowGlow cachedGlow;
    private WeatherDef weatherDef;
    private float sunStrength;

    public float WindowViewBeauty {
      get {
        return BeautyUtility.AverageBeautyPerceptible(GetBestWindowViewCell() == viewCell1 ? viewCell2 : viewCell1, Map);
      }
    }

    private CellRect SkyAreaNorth {
      get {
        IntVec3 pos = Position + IntVec3.North;
        return new CellRect() {
          minX = (pos + IntVec3.West).x,
          minZ = pos.z,
          maxX = (pos + IntVec3.East).x,
          maxZ = (pos + new IntVec3(0, 0, 2)).z
        };
      }
    }

    private CellRect SkyAreaSouth {
      get {
        IntVec3 pos = Position + IntVec3.South;
        return new CellRect() {
          minX = (pos + IntVec3.West).x,
          minZ = (pos + new IntVec3(0, 0, -2)).z,
          maxX = (pos + IntVec3.East).x,
          maxZ = pos.z
        };
      }
    }

    private CellRect SkyAreaEast {
      get {
        IntVec3 pos = Position + IntVec3.East;
        return new CellRect() {
          minX = (pos + new IntVec3(0, 0, -2)).x,
          minZ = (pos + IntVec3.South).z,
          maxX = pos.x,
          maxZ = (pos + IntVec3.North).z
        };
      }
    }

    private CellRect SkyAreaWest {
      get {
        IntVec3 pos = Position + IntVec3.West;
        return new CellRect() {
          minX = pos.x,
          minZ = (pos + IntVec3.South).z,
          maxX = (pos + new IntVec3(0, 0, 2)).x,
          maxZ = (pos + IntVec3.North).z
        };
      }
    }

    private float FactoredSunlight {
      get { return Map.skyManager.CurSkyGlow * sunStrength; }
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
      base.SpawnSetup(map, respawningAfterLoad);

      // Assign position info based on this window's rotation
      AssignWindowSkyAreas();

      // Get the inside cell, and spawn a glower there
      // This allows light to pass through the window without forcing one side to be indoors - things change
      // The JobDriver will still check for indoors/outdoors to prevent pawns from looking into other rooms
      viewCell = GetBestWindowViewCell();

      // If the view cell has changed to be on top of the glower, destroy the glower
      CheckForGlowerCellMismatch(viewCell);

      // These actions are only needed if there isn't already a glower
      if (glower == null) {
        SpawnGlower(viewCell);

        // Set the glower to use the cellRect on the opposite side of the window
        glowerCellRect = (viewCell == viewCell1) ? cellRect2 : cellRect1; 
      }
    }


    public override void DeSpawn() {
      if (glower != null) {
        glower.Destroy();
        glower = null;
      }
      base.DeSpawn();
    }


    public override void TickRare() {
      base.TickRare();

      viewCell = GetBestWindowViewCell();
      CheckForGlowerCellMismatch(viewCell);

      if (glower == null) {
        SpawnGlower(viewCell);
        glowerCellRect = (viewCell == viewCell1) ? cellRect2 : cellRect1;
      }

      GetSunlight();

      cachedSunlight = FactoredSunlight;
      cachedGlow = GlowStats();
      UpdateGlower(cachedGlow);
    }


    private void CheckForGlowerCellMismatch(IntVec3 c) {
      if (glower != null && c.GetThingList(Map).Find(t => t.def == ThingDef.Named("POW_WindowGlower")) == null) {
        glower.Destroy();
        glower = null;
      }
    }


    private void AssignWindowSkyAreas() {
      if (Rotation == Rot4.North || Rotation == Rot4.South) {
        cellRect1 = SkyAreaNorth;
        viewCell1 = Position + IntVec3.North;
        cellRect2 = SkyAreaSouth;
        viewCell2 = Position + IntVec3.South;
      }
      else {
        cellRect1 = SkyAreaEast;
        viewCell1 = Position + IntVec3.East;
        cellRect2 = SkyAreaWest;
        viewCell2 = Position + IntVec3.West;
      }
    }


    // There is potential here for a PeepingTom trait, but that would require too many additional features to be viable
    // (sex orientation, checking for bedroom, being spotted, social fights, etc. This may be an interesting addition, though)
    public IntVec3 GetBestWindowViewCell() {
      if (cellRect1 == null || cellRect2 == null) {
        Log.Warning("Powerless:: Tried to get a window view cell before cellRects were assigned. Assigning them now.");
        AssignWindowSkyAreas();
      }

      return GetSkyClearance(cellRect1) > GetSkyClearance(cellRect2) ? viewCell2 : viewCell1;
    }


    private float GetSkyClearance(CellRect rect) {
      int blockedCells = 0;
      // Iterate through the cells, counting any clear cells
      foreach (IntVec3 c in rect) {
        // If this cell is blocked, count it
        if (Map.roofGrid.Roofed(c)) {
          blockedCells++;
          continue;
        }
        List<Thing> thingsInCell = Map.thingGrid.ThingsListAtFast(c);
        for (int t = 0; t < thingsInCell.Count; t++) {
          if (thingsInCell[t].def.passability == Traversability.Impassable || thingsInCell[t].def.blockLight || thingsInCell[t].def.blockWind) {
            blockedCells++;
            break;
          }
        }
      }

      // Return the amount of clearance
      return (9f - blockedCells) / 9f;
    }


    private void SpawnGlower(IntVec3 pos) {
      // Create the glower and assign it to this faction
      glower = ThingMaker.MakeThing(ThingDef.Named("POW_WindowGlower")) as Building;
      glower.SetFactionDirect(Faction);
      // Wipe any existing glowers that may be present
      GenSpawn.WipeExistingThings(pos, Rot4.North, glower.def, Map, DestroyMode.Vanish);
      // Spawn the glower
      GenSpawn.Spawn(glower, pos, Map);
    }


    private void UpdateGlower(WindowGlow glow) {

      CompGlower glowComp = glower.GetComp<CompGlower>();

      float clearance = GetSkyClearance(glowerCellRect);

      // Change the color and radius based on sky colors and clearance
      glowComp.Props.glowColor = glow.color * clearance;
      glowComp.Props.glowRadius = glow.radius * clearance;

      // If the WindowGlow is overlit, add an overlightRadius
      // This is useful for building greenhouses
      if (glow.overlit && clearance >= 0.7f) {
        glowComp.Props.overlightRadius = 2.2f * clearance;
      }
      else {
        glowComp.Props.overlightRadius = 0f;
      }

      // Finalize the update
      Map.mapDrawer.MapMeshDirty(glower.Position, MapMeshFlag.Buildings);
      Map.glowGrid.MarkGlowGridDirty(glower.Position);
    }


    private void GetSunlight() {
      // Get the current weather
      weatherDef = Map.weatherManager.curWeather;

      // Clear weather provides the maximum sunlight
      if (weatherDef == WeatherDef.Named("Clear")) {
        sunStrength = 1f;
        return;
      }
      // These weathers provide 60% sunlight
      else if (weatherDef == WeatherDef.Named("Fog") ||
               weatherDef == WeatherDef.Named("Rain") ||
               weatherDef == WeatherDef.Named("SnowGentle")) {
        sunStrength = 0.6f;
        return;
      }
      // These weathers get only 25% sunlight
      else if (weatherDef == WeatherDef.Named("FoggyRain") ||
               weatherDef == WeatherDef.Named("SnowHard") ||
               weatherDef == WeatherDef.Named("DryThunderstorm") ||
               weatherDef == WeatherDef.Named("RainyThunderstorm")) {
        sunStrength = 0.25f;
        return;
      }
      // Default variable. Prevents issues when other mods add custom weather
      else {
        sunStrength = 1f;
      }
    }


    private WindowGlow GlowStats() {
      if (cachedSunlight >= 0.9f) {
        return bright;
      }
      if (cachedSunlight >= 0.72f) {
        return lit_5;
      }
      if (cachedSunlight >= 0.54f) {
        return lit_4;
      }
      if (cachedSunlight >= 0.36f) {
        return lit_3;
      }
      if (cachedSunlight >= 0.18f) {
        return lit_2;
      }
      if (cachedSunlight >= 0.08f) {
        return lit_1;
      }
      return dark;
    }
  }
}
