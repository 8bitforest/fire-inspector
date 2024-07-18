using FireInspector.Attributes.Validation;
using FireInspector.Editor.Elements;
using FireInspector.Utils;
using FireInspector.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FireInspector.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FireValidationAttribute), true)]
    public class ValidationDrawer : FirePropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            var errorsContainer = new VisualElement();

            var container = new VisualElement();
            container.Add(propertyField);
            container.Add(errorsContainer);

            UpdateErrorMessage(property, errorsContainer);
            propertyField.RegisterValueChangeCallback(_ => { UpdateErrorMessage(property, errorsContainer); });

            return container;
        }

        private void UpdateErrorMessage(SerializedProperty property, VisualElement errorsContainer)
        {
            errorsContainer.Clear();

            var validationAttribute = attribute as FireValidationAttribute;
            var issues = validationAttribute!.Validator.Validate(new InspectorProperty(property));

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