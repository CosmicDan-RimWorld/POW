using Verse;

namespace Powerless {

  public struct WindowGlow {

    public ColorInt color;
    public float radius;
    public bool overlit;


    public WindowGlow(ColorInt color, float radius, bool overlit = false) {
      this.color = color;
      this.radius = radius;
      this.overlit = overlit;
    }
  }
}
