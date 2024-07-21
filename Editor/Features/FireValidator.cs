using System.Collections.Generic;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;

namespace FireInspector.Editor.Features
{
    public abstract class FireValidator
    {
        public abstract IEnumerable<ValidationIssue> Validate(InspectorProperty property, object obj);
    }

    public abstract class AttributeValidator<T> : FireValidator where T : IFireValidationAttribute
    {
        public abstract IEnumerable<ValidationIssue> Validate(InspectorProperty property, T attribute);

        public override IEnumerable<ValidationIssue> Validate(InspectorProperty property, object obj)
        {
            return Validate(property, (T)obj);
        }
    }
}