using UnityEngine;
using Verse;

namespace Powerless {

  public enum SmokeStyle {
    None,
    Single,
    Triple
  }



  public class CompProperties_Smoker : CompProperties {

    public SmokeStyle smokeStyle = SmokeStyle.None;
    public int frequency = 60;
    public float size = 1f;
    public Vector3 offset = new Vector3(0.5f, 0, 0.5f);


    public CompProperties_Smoker() {
      compClass = typeof(CompSmoker);
    }
  }
}
