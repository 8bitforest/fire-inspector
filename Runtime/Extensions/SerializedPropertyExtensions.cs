using System.Linq;
using System.Reflection;
using UnityEditor;

namespace FireInspector.Extensions
{
    public static class SerializedPropertyExtensions
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
    }
}