using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Editor.Extensions
{
    internal static class SerializedPropertyExtensions
    {
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var type = targetObject.GetType();
            var path = property.propertyPath;
            FieldInfo fieldInfo = null;

            var parts = path.Split('.');
            foreach (var part in parts)
            {
                fieldInfo = type.GetField(part, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo == null)
                    return null;

                type = fieldInfo.FieldType;
            }

            return fieldInfo;
        }

        public static object GetGenericValue(this SerializedProperty property)
        {
            object targetObject = property.serializedObject.targetObject;
            var path = property.propertyPath;
            var parts = path.Split('.');
            foreach (var part in parts)
            {
                if (targetObject == null)
                    return null;

                var fieldInfo = targetObject.GetType().GetField(part,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo == null)
                    return null;

                targetObject = fieldInfo.GetValue(targetObject);
            }

            return targetObject;
        }

        public static object GetContainingObject(this SerializedProperty property)
        {
            object targetObject = property.serializedObject.targetObject;
            var path = property.propertyPath;
            var parts = path.Split('.');
            foreach (var part in parts.Take(parts.Length - 1))
            {
                if (targetObject == null)
                    return null;

                var fieldInfo = targetObject.GetType().GetField(part,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo == null)
                    return null;

                targetObject = fieldInfo.GetValue(targetObject);
            }

            return targetObject;
        }

        public static SerializedProperty FindSiblingProperty(this SerializedProperty property, string siblingName)
        {
            var parentPath = property.propertyPath[..property.propertyPath.LastIndexOf('.')];
            return property.serializedObject.FindProperty(parentPath + "." + siblingName);
        }

        public static bool IsValueEmpty(this SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (string.IsNullOrEmpty(property.stringValue))
                    return true;
            }
            else if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue == null || property.objectReferenceValue.Equals(null))
                    return true;
            }

            return false;
        }

        public static T GetAttribute<T>(this SerializedProperty property) where T : PropertyAttribute
        {
            return property.GetFieldInfo()?.GetCustomAttribute<T>();
        }

        public static T[] GetAttributes<T>(this SerializedProperty property)
        {
            return property.GetFieldInfo()?.GetCustomAttributes(typeof(T), true) as T[] ?? Array.Empty<T>();
        }
        
        public static int GetArraySize(this SerializedProperty property)
        {
            var sizeProperty = property.FindPropertyRelative("Array.size");
            return sizeProperty.intValue;
        }
    }
}