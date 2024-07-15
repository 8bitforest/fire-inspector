using System;
using System.Reflection;
using FireInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Utils
{
    public class InspectorProperty
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly SerializedObject _serializedObject;
        private readonly GameObject _gameObject;
        private readonly Component _component;

        public string ComponentName => _component.GetType().Name;
        public SerializedProperty Property { get; }
        public FieldInfo FieldInfo { get; }

        public string Name => Property.displayName ?? FieldInfo.Name;

        public InspectorProperty(SerializedProperty property)
        {
            Property = property;
            _serializedObject = property.serializedObject;
            if (_serializedObject is not { targetObject: Component component })
                throw new ArgumentException("SerializedObject target object is not a Component", nameof(property));

            _component = component;
            _gameObject = _component.gameObject;
            FieldInfo = property.GetFieldInfo();
        }

        public InspectorProperty(Component component, FieldInfo field)
        {
            _serializedObject = new SerializedObject(component);
            Property = _serializedObject.FindProperty(field.Name);
            _gameObject = component.gameObject;
            _component = component;
            FieldInfo = field;
        }

        public T[] GetAttributes<T>() where T : PropertyAttribute
        {
            return FieldInfo.GetCustomAttributes(typeof(T), true) as T[];
        }

        public string GetObjectPath()
        {
            var scene = _gameObject.scene;
            if (scene.IsValid())
            {
                var sceneName = scene.name;
                var objectPath = GetObjectPath(_gameObject.transform);
                return $"[{sceneName}] {objectPath}";
            }

            return GetObjectPath(_gameObject.transform);
        }

        private static string GetObjectPath(Transform current)
        {
            if (current.parent == null)
                return "/" + current.name;
            return GetObjectPath(current.parent) + "/" + current.name;
        }
    }
}