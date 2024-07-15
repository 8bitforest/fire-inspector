using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Utils
{
    public class InspectorProperty
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _serializedProperty;
        private readonly GameObject _gameObject;
        private readonly Component _component;
        private readonly FieldInfo _fieldInfo;

        public string ComponentName => _component.GetType().Name;
        public string Name => _serializedProperty?.displayName ?? _fieldInfo.Name;
        public object Value => _fieldInfo.GetValue(_component);

        public InspectorProperty(SerializedProperty property)
        {
            _serializedProperty = property;
            _serializedObject = property.serializedObject;
            _component = (_serializedObject.targetObject as Component)!;
            _gameObject = _component.gameObject;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var targetType = _component.GetType();
            _fieldInfo = targetType.GetField(property.propertyPath, flags);
        }

        public InspectorProperty(Component component, FieldInfo field)
        {
            _serializedObject = new SerializedObject(component);
            _serializedProperty = _serializedObject.FindProperty(field.Name);
            _gameObject = component.gameObject;
            _component = component;
            _fieldInfo = field;
        }

        public T[] GetAttributes<T>() where T : PropertyAttribute
        {
            return _fieldInfo.GetCustomAttributes(typeof(T), true) as T[];
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