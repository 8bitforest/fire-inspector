using System;
using System.Collections.Generic;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Extensions;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class FirePropertyDrawer : PropertyDrawer, IDisposable
    {
        private static readonly Dictionary<string, FirePropertyInfo> Properties = new();

        protected FirePropertyInfo Info => Properties[_propertyKey];
        private SerializedProperty _property;
        private string _propertyKey;
        private VisualElement _field;
        private VisualElement _errorsContainer;
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _property = property;
            _propertyKey = GetPropertyKey(property);

            var onChange = new Action(() =>
            {
                if (Properties.TryGetValue(_propertyKey, out var info))
                    info.InvokeChanged();
            });

            _field = CreateFieldElement(property, onChange);

            if (Properties.ContainsKey(_propertyKey))
                return _field;

            var info = new FirePropertyInfo(property);
            Properties.Add(_propertyKey, info);
            info.OnChanged(Update);

            _errorsContainer = new VisualElement();
            Update();

            var container = new VisualElement();
            container.Add(_field);
            container.Add(_errorsContainer);

            return container;
        }

        protected virtual VisualElement CreateFieldElement(SerializedProperty property, Action onChange)
        {
            var field = new PropertyField(property);
            field.RegisterValueChangeCallback(_ => onChange());
            return field;
        }

        protected void Update()
        {
            _errorsContainer.Clear();

            var issues = ProjectValidator.ValidateProperty(new InspectorProperty(_property), false);
            if (issues == null) return;

            foreach (var issue in issues)
            {
                var type = issue.IssueSeverity switch
                {
                    ValidationIssue.Severity.Info => InspectorPropertyMessage.MessageType.Info,
                    ValidationIssue.Severity.Warning => InspectorPropertyMessage.MessageType.Warning,
                    ValidationIssue.Severity.Error => InspectorPropertyMessage.MessageType.Error,
                    _ => InspectorPropertyMessage.MessageType.Info
                };
                var errorContainer = new InspectorPropertyMessage(issue.Message, type);
                _errorsContainer.Add(errorContainer);
            }
        }

        protected FirePropertyInfo FindProperty(string propertyName)
        {
            var property = _property.FindSiblingProperty(propertyName);
            if (property == null)
                return null;
            return Properties[GetPropertyKey(property)];
        }

        private string GetPropertyKey(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() + property.propertyPath;
        }

        public void Dispose()
        {
            if (_propertyKey == null) return;
            Properties.Remove(_propertyKey);
        }

        protected class FirePropertyInfo
        {
            public SerializedProperty Property { get; }
            public object Value => Property.boxedValue;

            private readonly List<Action> _onChangeCallbacks = new();

            public FirePropertyInfo(SerializedProperty property)
            {
                Property = property;
            }

            public void InvokeChanged()
            {
                foreach (var callback in _onChangeCallbacks)
                    callback();
            }

            public void OnChanged(Action callback)
            {
                _onChangeCallbacks.Add(callback);
            }
        }
    }
}