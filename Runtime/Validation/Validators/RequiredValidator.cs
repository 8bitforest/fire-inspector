using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Validation;
using FireInspector.Utils;
using UnityEditor;

namespace FireInspector.Validation.Validators
{
    public class RequiredValidator : IValidator
    {
        public IEnumerable<ValidationIssue> Validate(InspectorProperty property)
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
                return new[] { ValidationIssue.NotSupported(property, new RequiredAttribute()) };
            }

            return Enumerable.Empty<ValidationIssue>();
        }
    }
}