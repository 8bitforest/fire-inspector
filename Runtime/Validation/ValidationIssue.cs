using FireInspector.Attributes.Validation;
using FireInspector.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace FireInspector.Validation
{
    public class ValidationIssue
    {
        public enum Severity { Info, Warning, Error }

        public Severity IssueSeverity { get; private set; }
        [CanBeNull] public InspectorProperty Property { get; private set; }
        [CanBeNull] public GameObject GameObject { get; private set; }
        public string Message { get; private set; }

        public static ValidationIssue Info(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Info,
                Property = property,
                GameObject = property.GameObject,
                Message = message
            };
        }

        public static ValidationIssue Info(GameObject gameObject, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Info,
                GameObject = gameObject,
                Message = message
            };
        }

        public static ValidationIssue Warning(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Warning,
                Property = property,
                GameObject = property.GameObject,
                Message = message
            };
        }

        public static ValidationIssue Warning(GameObject gameObject, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Warning,
                GameObject = gameObject,
                Message = message
            };
        }

        public static ValidationIssue Error(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Error,
                Property = property,
                GameObject = property.GameObject,
                Message = message
            };
        }

        public static ValidationIssue Error(GameObject gameObject, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Error,
                GameObject = gameObject,
                Message = message
            };
        }

        public static ValidationIssue NotSupported(InspectorProperty property, FireValidationAttribute attribute)
        {
            var attributeName = attribute.GetType().Name;
            attributeName = attributeName.Substring(0, attributeName.Length - 9);

            return new ValidationIssue
            {
                IssueSeverity = Severity.Error,
                Property = property,
                Message =
                    $"{property.Name} ({property.Property.type}) does not support the [{attributeName}] attribute."
            };
        }
    }
}