using UnityEngine;
using System;

namespace Bazinga.Extensions
{
  [AttributeUsage(AttributeTargets.Field, Inherited = true)]
  public class InactiveAttribute : PropertyAttribute { }
}
