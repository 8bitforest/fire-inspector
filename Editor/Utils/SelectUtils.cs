using System.Collections.Generic;
using System.Reflection;
using FireInspector.Attributes.Properties;
using FireInspector.Editor.Extensions;
using FireInspector.Elements;
using UnityEditor;

namespace FireInspector.Editor.Utils
{
    public static class SelectUtils
    {
        public static MethodInfo GetOptionsMethod(SerializedProperty property)
        {
            var obj = property.GetContainingObject();
            var attribute = property.GetAttribute<SelectAttribute>();
            var methodName = attribute!.GetOptionsMethodName;
            var method = obj!.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return method;
        }

        public static List<SelectOption> GetSelectOptions(SerializedProperty property)
        {
            var obj = property.GetContainingObject();
            var method = GetOptionsMethod(property);
            var items = method!.Invoke(obj, null) as IEnumerable<SelectOption>;
            return items != null ? new List<SelectOption>(items) : new List<SelectOption>();
        }

        public static List<SelectOption<T>> GetSelectOptions<T>(SerializedProperty property)
        {
            var obj = property.GetContainingObject();
            var method = GetOptionsMethod(property);
            var items = method!.Invoke(obj, null) as IEnumerable<SelectOption<T>>;
            return items != null ? new List<SelectOption<T>>(items) : new List<SelectOption<T>>();
        }
    }
}