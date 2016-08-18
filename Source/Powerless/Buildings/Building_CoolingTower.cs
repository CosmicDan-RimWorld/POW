using System.Collections.Generic;
using System.Text;

using CorePanda;
using UnityEngine;
using RimWorld;
using Verse;

namespace Powerless {
  [StaticConstructorOnStartup]
  public class Building_CoolingTower : Building {

    // Comps and weather
    private CompSunlight sunlightComp;
    private CompRainTank raintankComp;
    private CompColdPusher coldpusherComp;
    private CompBreakdownable breakdownableComp;

    private IntVec3 roomLoc;            // Location of the tile in front of the tower
    private bool ventOpen = true;       // Is the vent open?
    private bool canFill = false;       // Does the player allow this tower to be filled with buckets?
    private bool transparent = false;   // Is the tower currently transparent?
    private string windSpeed;           // Used for inspect data, how windy it is outside
    
    // Graphic data
    private readonly Color R_TransColor = new Color(1f, 1f, 1f, 0.75f);
    private static readonly Vector2  S_BarSize = new Vector2(0.5f, 0.14f);
    private static readonly Material S_BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.325f, 0.65f, 1f));
    private static readonly Material S_BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));
    private Texture2D ventTex {
      get {
        if (ventOpen) {
          return ContentFinder<Texture2D>.Get("Cupro/UI/Designators/VentOpen", false);
        }
        else {
          return ContentFinder<Texture2D>.Get("Cupro/UI/Designators/VentClosed", false);
        }
      }
    }
    private Texture2D fillTex {
      get { return ContentFinder<Texture2D>.Get("Cupro/Item/Material/WaterBucket", false); }
    }

    // Water variables
    private const float C_MaxWater = 25000f;
    private const float C_WaterUsage = 0.04f;
    private readonly float R_BucketAmount = 1200f;
    private float waterUsage {
      get { return C_WaterUsage * sunlightComp.SimpleFactoredSunlight; }
    }

    public bool CanAcceptBuckets {
      get { return canFill && ((raintankComp.WaterLevel + R_BucketAmount) <= C_MaxWater); }
    }

    public int EmptySpace {
      get { return Mathf.FloorToInt((C_MaxWater - raintankComp.WaterLevel) / R_BucketAmount); }
    }

    // Set colors for transparent mode
    public override Color DrawColor {
      get {
        if (transparent) {
          return base.DrawColor * R_TransColor;
        }
        return base.DrawColor;
      }
      set { base.DrawColor = value; }
    }


    // Handle loading data
    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref ventOpen, "POW_CoolingTower_ventOpen", true);
      Scribe_Values.LookValue(ref canFill, "POW_CoolingTower_canFill", true);
    }


    public override void SpawnSetup() {
      base.SpawnSetup();

      // Register comps and variables
      sunlightComp = GetComp<CompSunlight>();
      raintankComp = GetComp<CompRainTank>();
      coldpusherComp = GetComp<CompColdPusher>();
      breakdownableComp = GetComp<CompBreakdownable>();

      sunlightComp.GetSunlight();
      roomLoc = Position + IntVec3.South.RotatedBy(Rotation);
      raintankComp.WaterLevelMax = C_MaxWater;
    }


    public override void Tick() {
      // Add water if it's raining, using the default
      // divisor since this happens every tick
      raintankComp.AddWater();

      // Determine current sunlight, but not every tick
      if (Find.TickManager.TicksGame % 50 == 0) {
        sunlightComp.GetSunlight();
      }

      // if the tower isn't broken down, and the room is hotter than it should be, and the vent is open, and there is enough water to evaporate,
      if ((breakdownableComp != null && !this.IsBrokenDown()) && roomLoc.GetTemperature() > coldpusherComp.Props.ColdPushMinTemperature && ventOpen && raintankComp.WaterLevel >= waterUsage) {
        // factor the strength of cold air, push it, and use water
        float strength = coldpusherComp.Props.ColdPerSecond * sunlightComp.SimpleFactoredSunlight * Mathf.Max(0.25f, WindManager.WindSpeed);
        coldpusherComp.Push(roomLoc, strength);
        raintankComp.UseWater(waterUsage);
      }

      // Reset the transparency
      if (Find.TickManager.TicksGame % 5000 == 0 && transparent) {
        transparent = false;
        Notify_ColorChanged();
        Find.MapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
      }
    }


    public void Notify_FilledWithBucket(int numBuckets = 1) {
      raintankComp.AddWaterDirect(R_BucketAmount * numBuckets);
    }


    // Handle the water meter graphic
    public override void Draw() {
      base.Draw();
      GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
      r.center = DrawPos + Vector3.up + (Vector3.forward * 0.5f);
      r.size = S_BarSize;
      r.fillPercent = raintankComp.WaterLevel / raintankComp.WaterLevelMax;
      r.filledMat = S_BarFilledMat;
      r.unfilledMat = S_BarUnfilledMat;
      r.margin = 0.15f;
      r.rotation = Rot4.East;
      GenDraw.DrawFillableBar(r);
    }


    // Add buttons for toggling vent or filling
    public override IEnumerable<Gizmo> GetGizmos() {
      Command_Toggle ventStatus = new Command_Toggle() {
        icon = ventTex,
        defaultDesc = "CP_ToggleVent".Translate(),
        hotKey = KeyBindingDefOf.CommandTogglePower,
        activateSound = SoundDef.Named("Click"),
        isActive = () => ventOpen,
        toggleAction = () => { ventOpen = !ventOpen; },
      };
      yield return ventStatus;

      Command_Toggle togFilling = new Command_Toggle() {
        icon = fillTex,
        defaultLabel = "POW_ToggleFill".Translate(),
        defaultDesc = "POW_ToggleFillDesc".Translate(),
        activateSound = SoundDef.Named("Click"),
        isActive = () => canFill,
        toggleAction = () => { canFill = !canFill; },
      };
      yield return togFilling;

      Command_Toggle togTrans = new Command_Toggle() {
        icon = ContentFinder<Texture2D>.Get("UI/Overlays/TargetHighlight_Square", false),
        defaultLabel = "POW_ToggleTrans".Translate(),
        defaultDesc = "POW_ToggleTransDesc".Translate(),
        activateSound = SoundDef.Named("Click"),
        isActive = () => transparent,
        toggleAction = () => {
          transparent = !transparent;
          Notify_ColorChanged();
          Find.MapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
        },
      };
      yield return togTrans;

      if (base.GetGizmos() != null) {
        foreach (Command c in base.GetGizmos()) {
          yield return c;
        }
      }
    }


    // Build the information panel
    public override string GetInspectString() {
      StringBuilder stringBuilder = new StringBuilder();
      // Get inherited string data
      stringBuilder.Append(base.GetInspectString());

      // Display the amount of water inside the tank
      stringBuilder.AppendLine("POW_WaterLevel".Translate() + ": " + raintankComp.WaterLevel.ToString("N0") + " / " + raintankComp.WaterLevelMax.ToString("N0"));

      // Determine how strong the wind is
      if (WindManager.WindSpeed < 0.4f) {
        windSpeed = "POW_GentleBreeze".Translate() + ".";
      }
      else if (WindManager.WindSpeed >= 0.4f && WindManager.WindSpeed < 0.8f) {
        windSpeed = "POW_ModerateBreeze".Translate() + ".";
      }
      else if (WindManager.WindSpeed >= 0.8f) {
        windSpeed = "POW_StrongWinds".Translate() + ".";
      }

      // Inform the player how the weather is affecting this building
      if (sunlightComp.WeatherLight == WeatherLight.Bright) {
        stringBuilder.Append("POW_ClearSkies".Translate() + " - ");
        stringBuilder.AppendLine(windSpeed);
      }
      else if (sunlightComp.WeatherLight == WeatherLight.Darkened) {
        stringBuilder.Append("POW_Overcast".Translate() + " - ");
        stringBuilder.AppendLine(windSpeed);
      }
      else if (sunlightComp.WeatherLight == WeatherLight.Dark) {
        stringBuilder.Append("POW_SevereOvercast".Translate() + " - ");
        stringBuilder.AppendLine(windSpeed);
      }

      // Display the current strength
      if (raintankComp.WaterLevel >= 0.1f) {
        stringBuilder.AppendLine("POW_CoolingStrength".Translate() + ": " + (Mathf.Round(sunlightComp.SimpleFactoredSunlight * Mathf.Max(0.25f, WindManager.WindSpeed) * 100)).ToString("##0") + "%");
      }
      else {
        stringBuilder.AppendLine("POW_CoolingStrength".Translate() + ": " + 0 + " (" + "POW_NoWater".Translate() + ")");
      } 

      return stringBuilder.ToString();
    }
  }
}