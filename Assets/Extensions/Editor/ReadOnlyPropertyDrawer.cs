using UnityEditor;
using UnityEngine;

namespace Bazinga.Extensions.Editor
{
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyPropertyDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      string valueStr;

      switch (property.propertyType)
      {
        case SerializedPropertyType.Integer:
          valueStr = property.intValue.ToString();
          break;
        case SerializedPropertyType.Boolean:
          valueStr = property.boolValue.ToString();
          break;
        case SerializedPropertyType.Float:
          valueStr = property.floatValue.ToString("0.00000");
          break;
        case SerializedPropertyType.String:
          valueStr = property.stringValue;
          break;
        case SerializedPropertyType.Vector3:
          valueStr = property.vector3Value.ToString();
          break;
        case SerializedPropertyType.Vector2:
          valueStr = property.vector2Value.ToString();
          break;
        case SerializedPropertyType.Vector4:
          valueStr = property.vector4Value.ToString();
          break;
        case SerializedPropertyType.Quaternion:
          valueStr = property.quaternionValue.ToString();
          break;
        default:
          valueStr = "(not supported)";
          break;
      }

      EditorGUI.BeginChangeCheck();
      EditorGUI.LabelField(position, label.text, valueStr);
    }
  }
}
