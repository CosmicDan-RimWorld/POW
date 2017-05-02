using System.Text;
using System.Collections.Generic;

using UnityEngine;
using RimWorld;
using Verse;

namespace Powerless {

  public class Building_CharcoalPit : Building {

    Building_CharPit charPit;

    // Using getters to allow for translations
    private string CharName {
      get {
        return ThingDef.Named("POW_Charcoal").LabelCap;
      }
    }


    public override void Tick() {
      base.Tick();

      // TODO: Casting as Building_CharPit may not return correct results, test this - esp burnTicks string
      charPit = Position.GetFirstThing(Map, ThingDef.Named("POW_CharPit")) as Building_CharPit;
      //charPit = Position.GetThingList(Map).Find(pitC => pitC is Building_CharPit) as Building_CharPit;
    }


    // Add buttons for charring wood
    public override IEnumerable<Gizmo> GetGizmos() {

      // If this location is able to hold a CharPit (not currently charring wood, no mystery blockages, etc.)
      if (GenConstruct.CanPlaceBlueprintAt(ThingDef.Named("POW_CharPit"), Position, Rotation, Map).Accepted) {

        // Add standard wood button
        Command_Action charcoal = new Command_Action() {
          icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
          defaultDesc = "POW_Char_D".Translate(),
          defaultLabel = "POW_Char_L".Translate(),
          activateSound = SoundDef.Named("Click"),
          action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Map, Rotation, Faction.OfPlayer, ThingDefOf.WoodLog); },
        };
        yield return charcoal;

        // If a mod adds bamboo, allow it to be charred
        if (DefDatabase<ThingDef>.GetNamed("Bamboo", false) != null) {
          Command_Action bamboo = new Command_Action() {
            icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
            defaultDesc = "POW_Bamb_D".Translate(),
            defaultLabel = "POW_Bamb_L".Translate(),
            activateSound = SoundDef.Named("Click"),
            action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Map, Rotation, Faction.OfPlayer, ThingDef.Named("Bamboo")); },
          };
          yield return bamboo;
        }

        // If ExpandedPower is installed, allow rubber wood to be charred
        if (DefDatabase<ThingDef>.GetNamed("EXP_RubberWood", false) != null) {
          Command_Action rubberWood = new Command_Action() {
            icon = ContentFinder<Texture2D>.Get("Cupro/UI/Commands/Charcoal", false),
            defaultDesc = "POW_Rubb_D".Translate(),
            defaultLabel = "POW_Rubb_L".Translate(),
            activateSound = SoundDef.Named("Click"),
            action = () => { GenConstruct.PlaceBlueprintForBuild(ThingDef.Named("POW_CharPit"), Position, Map, Rotation, Faction.OfPlayer, ThingDef.Named("EXP_RubberWood")); },
          };
          yield return rubberWood;
        }

      }

      // If there is currently a Char Pit in this location, allow it to be cancelled
      if (charPit != null) {
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
          },
        };
        yield return cancel;
      }

      // Yield any remaining gizmos
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

      return stringBuilder.ToString();
    }
  }
}
