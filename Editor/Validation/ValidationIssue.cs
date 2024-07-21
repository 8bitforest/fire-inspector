using FireInspector.Attributes.Validation;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace FireInspector.Editor.Validation
{
    public class ValidationIssue
    {
        public enum Severity { Info, Warning, Error }

        public Severity IssueSeverity { get; private set; }
        public string Message { get; private set; }
        [CanBeNull] public SerializedProperty Property { get; private set; }
        [CanBeNull] public Object Target { get; private set; }

        public static ValidationIssue Info(SerializedProperty property, string message)
            => New(property, Severity.Info, message);

        public static ValidationIssue Info(GameObject gameObject, string message)
            => New(gameObject, Severity.Info, message);

        public static ValidationIssue Warning(SerializedProperty property, string message)
            => New(property, Severity.Warning, message);

        public static ValidationIssue Warning(GameObject gameObject, string message)
            => New(gameObject, Severity.Warning, message);

        public static ValidationIssue Error(SerializedProperty property, string message)
            => New(property, Severity.Error, message);

        public static ValidationIssue Error(GameObject gameObject, string message)
            => New(gameObject, Severity.Error, message);

        private static ValidationIssue New(SerializedProperty property, Severity severity, string message)
        {
            // Copy the property since the property cannot be used if it has been moved past via Next()
            // This is how the Project Validator walks through the properties, so we need to handle it properly
            return new ValidationIssue
            {
                IssueSeverity = severity,
                Property = property.Copy(),
                Target = property.serializedObject.targetObject,
                Message = message
            };
        }

        private static ValidationIssue New(GameObject gameObject, Severity severity, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = severity,
                Target = gameObject,
                Message = message
            };
        }

        public static ValidationIssue NotSupported(SerializedProperty property, IFireValidationAttribute attribute)
        {
            var attributeName = attribute.GetType().Name;
            attributeName = attributeName.Substring(0, attributeName.Length - 9);

            return new ValidationIssue
            {
                IssueSeverity = Severity.Error,
                Property = property.Copy(),
                Target = property.serializedObject.targetObject,
                Message =
                    $"{property.displayName} ({property.type}) does not support the [{attributeName}] attribute."
            };
        }
    }
}