using System.Collections.Generic;
using System.Linq;
using FireInspector.Utils;

namespace FireInspector.Validation.Validators
{
    public class RequiredValidator : IValidator
    {
        public IEnumerable<ValidationIssue> Validate(InspectorProperty property)
        {
            var value = property.Value;
            if (value == null || value.Equals(null))
            {
                return new[]
                {
                    new ValidationIssue
                    {
                        IssueSeverity = ValidationIssue.Severity.Error,
                        Property = property,
                        Message = $"{property.Name} is required."
                    }
                };
            }

            return Enumerable.Empty<ValidationIssue>();
        }
    }
}