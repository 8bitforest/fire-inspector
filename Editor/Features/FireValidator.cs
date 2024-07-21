using System.Collections.Generic;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Validation;
using UnityEditor;

namespace FireInspector.Editor.Features
{
    public abstract class FireValidator
    {
        public abstract IEnumerable<ValidationIssue> Validate(SerializedProperty property, object obj);
    }

    public abstract class AttributeValidator<T> : FireValidator where T : IFireValidationAttribute
    {
        public abstract IEnumerable<ValidationIssue> Validate(SerializedProperty property, T attribute);

        public override IEnumerable<ValidationIssue> Validate(SerializedProperty property, object obj)
        {
            return Validate(property, (T)obj);
        }
    }
}