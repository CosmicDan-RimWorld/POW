using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace Powerless {

  public class CompSmoker : ThingComp {

    private Vector2 pSize;
    private Vector3 pPos;
    private Map pMap;
    private float offset;
    private int frequency;

    public CompProperties_Smoker Props {
      get { return (CompProperties_Smoker)props; }
    }


    public override void PostSpawnSetup() {
      base.PostSpawnSetup();

      // Cache parent info instead of constantly looking it up
      pSize = parent.def.size.ToVector2();
      pPos = parent.Position.ToVector3() + Props.offset;
      pMap = parent.Map;

      // Offset the smoke to balance triple smokes based on parent size
      // This likely won't look right for objects larger than a single tile
      offset = pSize.x / 3;

      frequency = Rand.RangeInclusive(Props.frequencyMin, Props.frequencyMax);
    }


    public override void CompTick() {
      base.CompTick();

      if (Find.TickManager.TicksGame % frequency == 0) {

        // Only throw motes if the location is rendered and valid
        if (!pPos.ShouldSpawnMotesAt(pMap) || pMap.moteCounter.SaturatedLowPriority) {
          return;
        }

        // Assign a new random frequency
        frequency = Rand.RangeInclusive(Props.frequencyMin, Props.frequencyMax);

        if (Props.smokeStyle == SmokeStyle.Single) {
          ThrowSmokeSingle();
        }
        if (Props.smokeStyle == SmokeStyle.Triple) {
          ThrowSmokeTriple();
        }
      }
    }


    private void ThrowSmokeSingle() {
      MoteMaker.ThrowSmoke(pPos, pMap, Props.size);
    }


    private void ThrowSmokeTriple() {
      MoteMaker.ThrowSmoke(pPos + new Vector3(-offset, 0, 0), pMap, Props.size);
      MoteMaker.ThrowSmoke(pPos,                              pMap, Props.size);
      MoteMaker.ThrowSmoke(pPos + new Vector3(offset, 0, 0),  pMap, Props.size);
    }
  }
}
