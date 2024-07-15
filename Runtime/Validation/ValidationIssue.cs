using FireInspector.Attributes;
using FireInspector.Utils;

namespace FireInspector.Validation
{
    public class ValidationIssue
    {
        public enum Severity { Info, Warning, Error }

        public Severity IssueSeverity { get; private set; }
        public InspectorProperty Property { get; private set; }
        public string Message { get; private set; }

        public static ValidationIssue Info(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Info,
                Property = property,
                Message = message
            };
        }

        public static ValidationIssue Warning(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Warning,
                Property = property,
                Message = message
            };
        }

        public static ValidationIssue Error(InspectorProperty property, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = Severity.Error,
                Property = property,
                Message = message
            };
        }

        public static ValidationIssue NotSupported(InspectorProperty property, ValidationAttribute attribute)
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