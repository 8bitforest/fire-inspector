using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Validation;
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
            var propertyType = property.propertyType;
            if (propertyType == SerializedPropertyType.String)
            {
                var value = property.stringValue;
                if (string.IsNullOrEmpty(value))
                    return new[] { ValidationIssue.Error(property, $"{property.displayName} is required.") };
            }
            else if (propertyType == SerializedPropertyType.ObjectReference)
            {
                var value = property.objectReferenceValue;
                if (value == null || value.Equals(null))
                    return new[] { ValidationIssue.Error(property, $"{property.displayName} is required.") };
            }
            else
            {
                return new[] { ValidationIssue.NotSupported(property, attribute) };
            }

            return Enumerable.Empty<ValidationIssue>();
        }
    }
}