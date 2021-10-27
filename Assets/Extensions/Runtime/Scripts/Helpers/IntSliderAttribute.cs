using System;

namespace Bazinga.Extensions
{
  [Serializable]
  public class IntSliderAttribute : Attribute
  {
    public int Min { get; private set; }
    public int Max { get; private set; }

    public IntSliderAttribute(int min, int max)
    {
      Min = min;
      Max = max;
    }
  }
}
