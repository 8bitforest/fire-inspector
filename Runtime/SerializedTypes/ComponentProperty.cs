using System;
using System.Linq;
using System.Reflection;
using FireInspector.Attributes.Properties;
using FireInspector.Attributes.Validation;
using FireInspector.Elements;
using UnityEngine;

namespace FireInspector.SerializedTypes
{
    [Serializable]
    public class ComponentProperty<T> : ISerializationCallbackReceiver
    {
        [SerializeField] [Required] [ComponentSelect]
        private Component component;

        [SerializeField] [Required] [Select(nameof(GetProperties), nameof(component))]
        private string property;

        public T Value => (T)_fieldInfo?.GetValue(component);

        private FieldInfo _fieldInfo;

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            if (component != null && !string.IsNullOrEmpty(property))
            {
                _fieldInfo = component?.GetType().GetField(property,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_fieldInfo == null)
                    Debug.LogError($"Field {property} not found on component {component?.GetType().Name}");
            }
        }

        private SelectOptionList<string> GetProperties()
        {
            if (component == null)
                return new SelectOptionList<string>();

            return new SelectOptionList<string>(component.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => typeof(T).IsAssignableFrom(f.FieldType))
                .Select(f => new SelectOption<string>($"{GetFriendlyTypeName(f.FieldType)} {f.Name}", f.Name)));
        }

        private string GetFriendlyTypeName(Type type)
        {
            var typeName = type.Name;
            if (type.IsGenericType)
            {
                var generics = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
                typeName = $"{typeName[..typeName.IndexOf('`')]}<{generics}>";
            }

            return typeName;
        }
    }
}