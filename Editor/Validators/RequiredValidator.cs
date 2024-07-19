using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;
using JetBrains.Annotations;
using UnityEditor;

namespace FireInspector.Editor.Validators
{
    [UsedImplicitly]
    public class RequiredValidator : AttributeValidator<RequiredAttribute>
    {
        public override IEnumerable<ValidationIssue> Validate(InspectorProperty property, RequiredAttribute attribute)
        {
            var propertyType = property.Property.propertyType;
            if (propertyType == SerializedPropertyType.String)
            {
                var value = property.Property.stringValue;
                if (string.IsNullOrEmpty(value))
                    return new[] { ValidationIssue.Error(property, $"{property.Name} is required.") };
            }
            else if (propertyType == SerializedPropertyType.ObjectReference)
            {
                var value = property.Property.objectReferenceValue;
                if (value == null || value.Equals(null))
                    return new[] { ValidationIssue.Error(property, $"{property.Name} is required.") };
            }
            else
            {
                return new[] { ValidationIssue.NotSupported(property, attribute) };
            }

            return Enumerable.Empty<ValidationIssue>();
        }
    }
}