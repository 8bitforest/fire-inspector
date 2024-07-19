using System;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FireInspector.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(IFireValidationAttribute), true)]
    public class FirePropertyDrawer : PropertyDrawer
    {
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var errorsContainer = new VisualElement();
            var field = CreateFieldElement(property, () => { UpdateErrorMessage(property, errorsContainer); });
            UpdateErrorMessage(property, errorsContainer);

            var container = new VisualElement();
            container.Add(field);
            container.Add(errorsContainer);

            return container;
        }

        protected virtual VisualElement CreateFieldElement(SerializedProperty property, Action onChange)
        {
            var field = new PropertyField(property);
            field.RegisterValueChangeCallback(_ => onChange());
            return field;
        }

        private void UpdateErrorMessage(SerializedProperty property, VisualElement errorsContainer)
        {
            errorsContainer.Clear();

            if (attribute is not IFireValidationAttribute validationAttribute) return;

            var issues =
                ProjectValidator.ValidatePropertyAttribute(new InspectorProperty(property), validationAttribute);
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
    }
}