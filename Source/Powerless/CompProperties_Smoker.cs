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
    public int frequency = 30;
    public float size = 1f;
    public Vector3 offset = new Vector3(0,0,0);


    public CompProperties_Smoker() {
      compClass = typeof(CompSmoker);
    }
  }
}
