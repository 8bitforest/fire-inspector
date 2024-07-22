using JetBrains.Annotations;
using UnityEngine;

namespace FireInspector.Validation
{
    public class ValidationIssue
    {
        public enum Severity { Info, Warning, Error }

        public Severity IssueSeverity { get; init; }
        public string Message { get; init; }
        [CanBeNull] public Object Target { get; init; }

        public static ValidationIssue Info(string message)
            => New(null, Severity.Info, message);

        public static ValidationIssue Info(Component component, string message)
            => New(component, Severity.Info, message);

        public static ValidationIssue Info(GameObject gameObject, string message)
            => New(gameObject, Severity.Info, message);

        public static ValidationIssue Warning(string message)
            => New(null, Severity.Warning, message);

        public static ValidationIssue Warning(Component component, string message)
            => New(component, Severity.Warning, message);

        public static ValidationIssue Warning(GameObject gameObject, string message)
            => New(gameObject, Severity.Warning, message);

        public static ValidationIssue Error(string message)
            => New(null, Severity.Error, message);

        public static ValidationIssue Error(Component component, string message)
            => New(component, Severity.Error, message);

        public static ValidationIssue Error(GameObject gameObject, string message)
            => New(gameObject, Severity.Error, message);

        private static ValidationIssue New(Object target, Severity severity, string message)
        {
            return new ValidationIssue
            {
                IssueSeverity = severity,
                Target = target,
                Message = message
            };
        }
    }
}