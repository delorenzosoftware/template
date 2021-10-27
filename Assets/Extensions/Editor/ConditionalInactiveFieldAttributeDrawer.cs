using UnityEditor;
using UnityEngine;

namespace Bazinga.Extensions.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalInactiveFieldAttribute))]
    public class ConditionalInactiveFieldAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is ConditionalInactiveFieldAttribute conditional)) return;

            bool enabled = false;

            if (!string.IsNullOrEmpty(conditional.FieldToCheck))
            {
                var propertyToCheck = ConditionalFieldUtility.FindRelativeProperty(property, conditional.FieldToCheck);
                enabled = !ConditionalFieldUtility.PropertyIsVisible(propertyToCheck, conditional.Inverse, conditional.CompareValues);
            }

            GUI.enabled = enabled;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
