using System;
using System.Collections.Generic;
using System.Reflection;
using FireInspector.Attributes.Other;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Extensions;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;
using FireInspector.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features
{
    [CustomEditor(typeof(object), true, isFallback = true)]
    public class FireEditor : UnityEditor.Editor, IDisposable
    {
        private static readonly Dictionary<int, FireEditorObject> Objects = new();
        private VisualElement _objectErrorsContainer;

        ~FireEditor()
        {
            Dispose();
        }

        public static FireEditorProperty FindEditorProperty(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var objectKey = targetObject.GetInstanceID();
            if (!Objects.ContainsKey(objectKey))
                return null;

            var propertyKey = property.propertyPath;
            if (!Objects[objectKey].Properties.ContainsKey(propertyKey))
                return null;

            return Objects[objectKey].Properties[propertyKey];
        }

        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            if (serializedObject.targetObject == null)
                return inspector;

            var objectKey = GetObjectKey(serializedObject);
            Objects.Remove(objectKey);
            Objects.Add(objectKey, new FireEditorObject());

            _objectErrorsContainer = new VisualElement();
            _objectErrorsContainer.AddToClassList("fire-inspector-object-errors-container");
            inspector.Add(_objectErrorsContainer);

            var scriptField = serializedObject.FindProperty("m_Script");
            if (scriptField != null)
                AddPropertyField(inspector, scriptField);

            AddObjectFields(inspector, serializedObject.targetObject);

            UpdateObjectErrors();

            return inspector;
        }

        private void AddObjectFields(VisualElement to, object obj, string path = "")
        {
            if (obj == null)
                return;

            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(HideInInspector), true))
                    continue;
                
                var property = serializedObject.FindProperty(path + field.Name);
                if (property != null)
                {
                    AddPropertyField(to, property);
                }
                else
                {
                    var attribute = field.GetCustomAttribute<ShowInInspectorAttribute>();
                    if (attribute != null)
                        to.Add(ShowInInspectorDrawer.CreateFieldGUI(serializedObject, field));
                }
            }
        }

        private void AddPropertyField(VisualElement inspector, SerializedProperty property)
        {
            PropertyField propertyField;

            // In the future, we could make sure the type doesn't have a custom drawer
            if (property.hasVisibleChildren && !property.isArray)
            {
                var foldout = new Foldout { text = property.displayName };
                AddObjectFields(foldout.contentContainer, property.GetGenericValue(), property.propertyPath + ".");

                propertyField = new PropertyField();
                propertyField.Add(foldout);
            }
            else
            {
                propertyField = new PropertyField(property);
            }

            propertyField.name = "PropertyField:" + property.propertyPath;
            if (property.propertyPath == "m_Script")
                propertyField.SetEnabled(false);

            var errorsContainer = new VisualElement();
            errorsContainer.AddToClassList("fire-inspector-errors-container");

            var container = new VisualElement();
            container.Add(propertyField);
            container.Add(errorsContainer);

            var objectKey = GetObjectKey(serializedObject);
            var propertyKey = GetPropertyKey(property);
            var editorProperty = new FireEditorProperty(property);
            Objects[objectKey].Properties.Remove(propertyKey);
            Objects[objectKey].Properties.Add(propertyKey, editorProperty);

            UpdatePropertyErrors(errorsContainer, property);
            propertyField.RegisterValueChangeCallback(_ => editorProperty.InvokeChanged());
            editorProperty.OnChanged(() =>
            {
                UpdatePropertyErrors(errorsContainer, property);
                UpdateObjectErrors();
            });

            inspector.Add(container);
        }

        private void UpdatePropertyErrors(VisualElement errorsContainer, SerializedProperty property)
        {
            errorsContainer.Clear();

            var issues = ProjectValidator.ValidateProperty(property);
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
                errorsContainer.Add(errorContainer);
            }
        }

        private void UpdateObjectErrors()
        {
            _objectErrorsContainer.Clear();

            var issues = ProjectValidator.ValidateObject(serializedObject.targetObject);
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
                _objectErrorsContainer.Add(errorContainer);
            }
        }

        public void Dispose()
        {
            Objects.Remove(GetObjectKey(serializedObject));
        }

        private static int GetObjectKey(SerializedObject obj)
        {
            return obj.targetObject.GetInstanceID();
        }

        private static string GetPropertyKey(SerializedProperty property)
        {
            return property.propertyPath;
        }

        private class FireEditorObject
        {
            public Dictionary<string, FireEditorProperty> Properties { get; } = new();
        }

        public class FireEditorProperty
        {
            public SerializedProperty Property { get; }
            private readonly List<Action> _onChangeCallbacks = new();

            public FireEditorProperty(SerializedProperty property)
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