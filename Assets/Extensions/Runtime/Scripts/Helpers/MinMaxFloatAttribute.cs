using System;

namespace Bazinga.Extensions
{
  [Serializable]
  public class MinMaxFloatAttribute : Attribute
  {
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMaxFloatAttribute(int min, int max)
    {
      Min = min;
      Max = max;
    }
  }
}