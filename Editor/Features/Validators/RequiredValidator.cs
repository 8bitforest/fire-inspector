using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Extensions;
using FireInspector.Editor.Validation;
using JetBrains.Annotations;
using UnityEditor;

namespace FireInspector.Editor.Features.Validators
{
    [UsedImplicitly]
    public class RequiredValidator : AttributeValidator<RequiredAttribute>
    {
        public override IEnumerable<ValidationIssue> Validate(SerializedProperty property, RequiredAttribute attribute)
        {
            var error = new[] { ValidationIssue.Error(property, $"{property.displayName} is required.") };
            var propertyType = property.propertyType;
            if (propertyType == SerializedPropertyType.String)
            {
                if (string.IsNullOrEmpty(property.stringValue)) return error;
            }
            else if (propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue == null) return error;
            }
            else if (property.isArray)
            {
                if (property.GetArraySize() == 0) return error;
            }
            else
            {
                return new[] { ValidationIssue.NotSupported(property, attribute) };
            }

            return Enumerable.Empty<ValidationIssue>();
        }
    }
}