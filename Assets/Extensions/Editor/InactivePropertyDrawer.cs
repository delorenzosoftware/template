using UnityEditor;
using UnityEngine;

namespace Bazinga.Extensions.Editor
{
  [CustomPropertyDrawer(typeof(InactiveAttribute))]
  public class InactivePropertyDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginChangeCheck();
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label);
      GUI.enabled = true;
    }
  }
}
