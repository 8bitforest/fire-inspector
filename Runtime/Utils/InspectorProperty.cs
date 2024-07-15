using System;
using System.Reflection;
using FireInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Utils
{
    public class InspectorProperty
    {
        public SerializedProperty Property { get; }
        public SerializedObject SerializedObject { get; }
        public FieldInfo FieldInfo { get; }
        public Component Component { get; }
        public GameObject GameObject { get; }

        public string Name => Property.displayName ?? FieldInfo.Name;

        public InspectorProperty(SerializedProperty property)
        {
            Property = property;
            SerializedObject = property.serializedObject;
            if (SerializedObject is not { targetObject: Component component })
                throw new ArgumentException("SerializedObject target object is not a Component", nameof(property));

            Component = component;
            GameObject = Component.gameObject;
            FieldInfo = property.GetFieldInfo();
        }

        public InspectorProperty(Component component, FieldInfo field)
        {
            SerializedObject = new SerializedObject(component);
            Property = SerializedObject.FindProperty(field.Name);
            GameObject = component.gameObject;
            Component = component;
            FieldInfo = field;
        }

        public T[] GetAttributes<T>() where T : PropertyAttribute
        {
            return FieldInfo.GetCustomAttributes(typeof(T), true) as T[];
        }
    }
}