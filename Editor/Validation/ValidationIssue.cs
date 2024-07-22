using FireInspector.Attributes.Validation;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Editor.Validation
{
    public class EditorValidationIssue : FireInspector.Validation.ValidationIssue
    {
        [CanBeNull] public SerializedProperty Property { get; private set; }

        public static EditorValidationIssue Info(SerializedProperty property, string message)
            => New(property, Severity.Info, message);

        public new static EditorValidationIssue Info(GameObject gameObject, string message)
            => New(gameObject, Severity.Info, message);

        public static EditorValidationIssue Warning(SerializedProperty property, string message)
            => New(property, Severity.Warning, message);

        public new static EditorValidationIssue Warning(GameObject gameObject, string message)
            => New(gameObject, Severity.Warning, message);

        public static EditorValidationIssue Error(SerializedProperty property, string message)
            => New(property, Severity.Error, message);

        public new static EditorValidationIssue Error(GameObject gameObject, string message)
            => New(gameObject, Severity.Error, message);

        private static EditorValidationIssue New(SerializedProperty property, Severity severity, string message)
        {
            // Copy the property since the property cannot be used if it has been moved past via Next()
            // This is how the Project Validator walks through the properties, so we need to handle it properly
            return new EditorValidationIssue
            {
                IssueSeverity = severity,
                Property = property.Copy(),
                Target = property.serializedObject.targetObject,
                Message = message
            };
        }

        private static EditorValidationIssue New(Object target, Severity severity, string message)
        {
            return new EditorValidationIssue
            {
                IssueSeverity = severity,
                Target = target,
                Message = message
            };
        }

        public static EditorValidationIssue NotSupported(SerializedProperty property,
            IFireValidationAttribute attribute)
        {
            var attributeName = attribute.GetType().Name;
            attributeName = attributeName.Substring(0, attributeName.Length - 9);

            return new EditorValidationIssue
            {
                IssueSeverity = Severity.Error,
                Property = property.Copy(),
                Target = property.serializedObject.targetObject,
                Message =
                    $"[{attributeName}] does not support {property.type}."
            };
        }
    }
}