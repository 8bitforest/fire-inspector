using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Properties;
using FireInspector.Editor.Extensions;
using FireInspector.Editor.Utils;
using FireInspector.Editor.Validation;
using FireInspector.Elements;
using JetBrains.Annotations;
using UnityEditor;

namespace FireInspector.Editor.Features.Validators
{
    [UsedImplicitly]
    public class SelectValidator : AttributeValidator<SelectAttribute>
    {
        public override IEnumerable<ValidationIssue> Validate(SerializedProperty property, SelectAttribute attribute)
        {
            var options = SelectUtils.GetSelectOptions(property);

            // Make sure that the types of the property and the options match
            var field = property.GetFieldInfo();
            var fieldType = field.FieldType;
            var method = SelectUtils.GetOptionsMethod(property);
            var returnType = method.ReturnType;
            if (returnType != typeof(SelectOptionList<>).MakeGenericType(fieldType))
            {
                return new[]
                {
                    ValidationIssue.Error(property,
                        $"Get options method should return a SelectOptionList<{fieldType}>.")
                };
            }

            // If it depends on another property, make sure that property exists
            var issues = ProjectValidator.ValidatePropertyReference(property, attribute.DependsOn);
            if (issues.Any()) return issues;

            // Let the RequiredValidator handle this, if the user wants to enforce a selection
            if (property.IsValueEmpty()) return null;

            // If a value *is* set, make sure it's a valid one
            var value = property.boxedValue;
            if (options.FirstOrDefault(o => o.Value.Equals(value)) == null)
                return new[] { ValidationIssue.Error(property, "Selection is no longer valid.") };

            return null;
        }
    }
}