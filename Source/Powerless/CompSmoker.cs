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
    private MoteThrown smokeMote;

    public CompProperties_Smoker Props {
      get { return (CompProperties_Smoker)props; }
    }


    public override void PostSpawnSetup() {
      base.PostSpawnSetup();

      smokeMote = ThingMaker.MakeThing(ThingDefOf.Mote_Smoke) as MoteThrown;

      // Cache parent info instead of constantly looking it up
      pSize = parent.def.size.ToVector2();
      pPos = parent.Position.ToVector3() + Props.offset;
      pMap = parent.Map;
    }


    public override void CompTick() {
      base.CompTick();

      if (Find.TickManager.TicksGame % Props.frequency == 0) {

        // Only throw motes if the map is rendered
        if (!pPos.ShouldSpawnMotesAt(parent.Map)) {
          return;
        }

        if (Props.smokeStyle == SmokeStyle.Single) {
          ThrowSmokeSingle(pPos);
        }
        if (Props.smokeStyle == SmokeStyle.Triple) {
          ThrowSmokeTriple();
        }
      }
    }


    private void ThrowSmokeSingle(Vector3 pos) {

      // Randomize the scale of the smoke around the desired size
      smokeMote.Scale = Rand.Range(0.8f, 1.3f) * Props.size;
      smokeMote.exactPosition = pos;
      smokeMote.rotationRate = Rand.Range(-20f, 20f);
      smokeMote.SetVelocity(Rand.Range(-20, 20), Rand.Range(0.3f, 0.5f));

      // Randomize the color a bit
      int randColor = Rand.RangeInclusive(10, 50);
      smokeMote.instanceColor = Color.black + new Color(randColor, randColor, randColor);

      GenSpawn.Spawn(smokeMote, pos.ToIntVec3(), pMap);
    }


    private void ThrowSmokeTriple() {

      // Offset the smoke to balance based on size
      // This likely won't look right for objects larger than a single tile
      float offset = pSize.x / 4;

      ThrowSmokeSingle(pPos + new Vector3(-offset, 0, 0));
      ThrowSmokeSingle(pPos);
      ThrowSmokeSingle(pPos + new Vector3(offset, 0, 0));
    }
  }
}
