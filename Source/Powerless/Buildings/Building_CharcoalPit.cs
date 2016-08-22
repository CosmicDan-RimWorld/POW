using System.Text;
using System.Collections.Generic;

using UnityEngine;
using RimWorld;
using Verse;

namespace Powerless {

  public class Building_CharcoalPit : Building {

    Building_CharPit charPit;
    Building_YakiPit yakiPit;

    // Using getters to allow for translations
    private string CharName {
      get {
        return ThingDef.Named("POW_Charcoal").LabelCap;
      }
    }
    private string YakiName {
      get {
        return ThingDef.Named("POW_Yakisugi").LabelCap;
      }
    }


    public override void Tick() {
      base.Tick();

      charPit = Position.GetThingList().Find(pitC => pitC is Building_CharPit) as Building_CharPit;
      yakiPit = Position.GetThingList().Find(pitY => pitY is Building_YakiPit) as Building_YakiPit;
    }


    // Add buttons for charring wood
    public override IEnumerable<Gizmo> GetGizmos() {

      if (GenConstruct.CanPlaceBlueprintAt(ThingDef.Named("POW_CharPit"), Position, Rotation).Accepted && 
          GenConstruct.CanPlaceBlueprintAt(ThingDef.Named("POW_YakiPit"), Position, Rotation).Accepted) {

        Command_Action charcoal = new Command_Action() {
          icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
          defaultDesc = "POW_Char_D".Translate(),
          defaultLabel = "POW_Char_L".Translate(),
          activateSound = SoundDef.Named("Click"),
          action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Rotation, Faction.OfPlayer, ThingDefOf.WoodLog); },
        };
        yield return charcoal;

        if (DefDatabase<ThingDef>.GetNamed("Bamboo", false) != null) {
          Command_Action bamboo = new Command_Action() {
            icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
            defaultDesc = "POW_Bamb_D".Translate(),
            defaultLabel = "POW_Bamb_L".Translate(),
            activateSound = SoundDef.Named("Click"),
            action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Rotation, Faction.OfPlayer, ThingDef.Named("Bamboo")); },
          };
          yield return bamboo;
        }

        if (DefDatabase<ThingDef>.GetNamed("EXP_RubberWood", false) != null) {
          Command_Action rubberWood = new Command_Action() {
            icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
            defaultDesc = "POW_Rubb_D".Translate(),
            defaultLabel = "POW_Rubb_L".Translate(),
            activateSound = SoundDef.Named("Click"),
            action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Rotation, Faction.OfPlayer, ThingDef.Named("EXP_RubberWood")); },
          };
          yield return rubberWood;
        }

        Command_Action yakisugi = new Command_Action() {
          icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Yakisugi", false),
          defaultDesc = "POW_Yaki_D".Translate(),
          defaultLabel = "POW_Yaki_L".Translate(),
          activateSound = SoundDef.Named("Click"),
          action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_YakiPit"), Position, Rotation, Faction.OfPlayer, ThingDefOf.WoodLog); },
        };
        yield return yakisugi;
      }

      if (charPit != null || yakiPit != null) {
        Command_Action cancel = new Command_Action() {
          icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", false),
          defaultDesc = "POW_PitCancel".Translate(),
          defaultLabel = "DesignatorCancel".Translate(),
          activateSound = SoundDef.Named("Click"),
          action = () => {
            if (charPit != null) {
              charPit.Destroy(DestroyMode.Deconstruct);
              charPit = null;
            }
            if (yakiPit != null) {
              yakiPit.Destroy(DestroyMode.Deconstruct);
              yakiPit = null;
            }
          },
        };
        yield return cancel;
      }

      foreach (Command c in base.GetGizmos()) {
        yield return c;
      }
    }


    public override string GetInspectString() {
      StringBuilder stringBuilder = new StringBuilder();

      // Display the burning progress
      if (charPit != null) {
        stringBuilder.AppendLine(CharName + " " + "CP_Progress".Translate().ToLower() + ": (" + (100f - (charPit.BurnTicks / 0.8f)).ToString("#0.00") + "%)");
      }
      if (yakiPit != null) {
        stringBuilder.AppendLine(YakiName + " " + "CP_Progress".Translate().ToLower() + ": (" + (100f - (yakiPit.BurnTicks / 0.3f)).ToString("#0.00") + "%)"); 
      }

      return stringBuilder.ToString();
    }
  }
}
