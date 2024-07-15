using FireInspector.Utils;

namespace FireInspector.Validation
{
    public class ValidationIssue
    {
        public enum Severity
        {
            Info,
            Warning,
            Error
        }
        
        public Severity IssueSeverity { get; set; }
        public InspectorProperty Property { get; set; }
        public string Message { get; set; }
    }
}