using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Component = UnityEngine.Component;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bazinga.Extensions
{
    public static class ExtensionMethods
    {
        public static T FindNextParent<T>(this Transform child)
        {
            var transformParent = child.parent;
            if (transformParent == null) return default;

            var artifactParent = transformParent.GetComponent<T>();

            if (artifactParent == null)
            {
                return FindNextParent<T>(transformParent);
            }
            else
            {
                return artifactParent;
            }
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static float GetScaledValue(this float value,
        float minValue, float maxValue,
        float minNorm = -10f, float maxNorm = 10f)
        {
            // Bounds normalization
            if (value > maxValue)
            {
                value = maxValue;
            }
            else if (value < minValue)
            {
                value = minValue;
            }

            //Scaling the output value
            float inputRange = maxValue - minValue;
            float outputRange = maxNorm - minNorm;
            float output = (((value - minValue) * outputRange) / inputRange) + minNorm;

            return output;
        }

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null;

            BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Default |
            BindingFlags.DeclaredOnly;

            PropertyInfo[] pinfos = type.GetProperties(flags);

            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        Debug.LogWarning("Copy properties failed!");
                    }
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);

            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

#if UNITY_EDITOR
        public static string AsStringValue(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;

                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (property.type == "char") return Convert.ToChar(property.intValue).ToString();
                    return property.intValue.ToString();

                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();

                case SerializedPropertyType.Enum:
                    return property.GetValue().ToString();

                default:
                    return string.Empty;
            }
        }

        public static object GetValue(this SerializedProperty property)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            var elements = property.GetFixedPropertyPath().Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                    obj = GetValueByArrayFieldName(obj, elementName, index);
                }
                else obj = GetValueByFieldName(obj, element);
            }
            return obj;


            object GetValueByArrayFieldName(object source, string name, int index)
            {
                if (!(GetValueByFieldName(source, name) is IEnumerable enumerable)) return null;
                var enumerator = enumerable.GetEnumerator();

                for (var i = 0; i <= index; i++) if (!enumerator.MoveNext()) return null;
                return enumerator.Current;
            }

            // Search "source" object for a field with "name" and get it's value
            object GetValueByFieldName(object source, string name)
            {
                if (source == null) return null;
                var type = source.GetType();

                while (type != null)
                {
                    var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (fieldInfo != null) return fieldInfo.GetValue(source);

                    var propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (propertyInfo != null) return propertyInfo.GetValue(source, null);

                    type = type.BaseType;
                }
                return null;
            }
        }

        public static string GetFixedPropertyPath(this SerializedProperty property) => property.propertyPath.Replace(".Array.data[", "[");
#endif
    }
}
