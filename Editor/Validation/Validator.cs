using System.Collections.Generic;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Utils;

namespace FireInspector.Editor.Validation
{
    public abstract class Validator
    {
        public abstract IEnumerable<ValidationIssue> Validate(InspectorProperty property, object obj);
    }

    public abstract class AttributeValidator<T> : Validator where T : IFireValidationAttribute
    {
        public abstract IEnumerable<ValidationIssue> Validate(InspectorProperty property, T attribute);

        public override IEnumerable<ValidationIssue> Validate(InspectorProperty property, object obj)
        {
            return Validate(property, (T)obj);
        }
    }
}