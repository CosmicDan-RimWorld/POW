﻿using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace Powerless {

  internal class JobDriver_LookOutWindow : JobDriver_WatchBuilding {


    protected override IEnumerable<Toil> MakeNewToils() {
      // TargetIndex.A is the window glower
      // TargetIndex.B is the window

      Building glower = TargetA.Thing as Building;
      Building_Window window = TargetB.Thing as Building_Window;

      // Set fail conditions
      this.FailOnDespawnedOrNull(TargetIndex.A);
      this.FailOnDestroyedOrNull(TargetIndex.B);

      // Reserve the window glower
      yield return Toils_Reserve.Reserve(TargetIndex.A);

      // Go to the window
      yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);

      // Look out the window
      Toil low = new Toil();
      JoyKindDef joyKind = DefDatabase<JobDef>.GetNamed("POW_Job_LookOutWindow").joyKind;
      low.socialMode = RandomSocialMode.Normal;
      low.tickAction = () => {
        base.WatchTickAction();
        if (glower != null) {
          pawn.needs.joy.GainJoy(Mathf.Max(window.WindowViewBeauty, 0f) * 0.000144f, joyKind);
        }
        pawn.Drawer.rotator.FaceCell(TargetB.Cell);
      };
      low.defaultCompleteMode = ToilCompleteMode.Delay;
      low.defaultDuration = CurJob.def.joyDuration;
      low.AddFinishAction(() => {
        // Create the basic memory
        Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDef.Named("POW_LookedOutWindowRegular"));

        // Create a temp list of thoughts the pawn has
        List<Thought> tmpThoughts = new List<Thought>(); ;
        pawn.needs.mood.thoughts.GetAllMoodThoughts(tmpThoughts);
        // Scan through the thoughts
        for (int t = 0; t < tmpThoughts.Count; t++) {
          // If the pawn has cabin fever, change the memory
          if (tmpThoughts[t].def.defName == "CabinFever") {
            thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDef.Named("POW_LookedOutWindowCabinFever"));
            // If the pawn has serious cabin fever, give a better memory
            if (tmpThoughts[t].CurStageIndex == 1) {
              thought_Memory.moodPowerFactor = 1.5f;
            }
          }
        }

        // Try to add the memory
        pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
      });
      yield return low;
    }


    public override string GetReport() {
      if (pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout)) {
        return "POW_WatchingToxicFallout".Translate();
      }
      if (pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.VolcanicWinter)) {
        return "POW_WatchingVolcanicWinter".Translate();
      }
      if (pawn.Map.weatherManager.RainRate > 0.1f) {
        return "POW_WatchingRain".Translate();
      }
      if (pawn.Map.weatherManager.SnowRate > 0.1f) {
        return "POW_WatchingSnow".Translate();
      }
      return base.GetReport();
    }
  }
}
