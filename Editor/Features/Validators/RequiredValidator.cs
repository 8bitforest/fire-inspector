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
        public override EditorValidationIssue Validate(SerializedProperty property, RequiredAttribute attribute)
        {
            var error = EditorValidationIssue.Error(property, $"{property.displayName} is required.");
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
                return EditorValidationIssue.NotSupported(property, attribute);
            }

            return null;
        }
    }
}