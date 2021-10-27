using UnityEditor;
using UnityEngine;

namespace Bazinga.Extensions.Editor
{
  [CustomPropertyDrawer(typeof(IntSlider), true)]
  public class IntSliderPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      label = EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, label);

      SerializedProperty valueProp = property.FindPropertyRelative("value");
      SerializedProperty minProp = property.FindPropertyRelative("min");
      SerializedProperty maxProp = property.FindPropertyRelative("max");

      int valueValue = valueProp.intValue;
      int minValue = minProp.intValue;
      int maxValue = maxProp.intValue;

      int valueMin = 0;
      int valueMax = 1;

      var ranges = (IntSliderAttribute[])fieldInfo.GetCustomAttributes(typeof(IntSliderAttribute), true);

      if (ranges.Length > 0)
      {
        valueMin = ranges[0].Min;
        valueMax = ranges[0].Max;
      }

      EditorGUI.BeginChangeCheck();

      valueValue = EditorGUI.IntSlider(position, valueValue, valueMin, valueMax);

      if (EditorGUI.EndChangeCheck())
      {
        valueProp.intValue = (int)valueValue;
        minProp.intValue = (int)minValue;
        maxProp.intValue = (int)maxValue;
      }

      EditorGUI.EndProperty();
    }
  }
}
