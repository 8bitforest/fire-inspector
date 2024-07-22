using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Validation;
using UnityEditor;

namespace FireInspector.Editor.Features
{
    public abstract class FireValidator
    {
        public abstract IEnumerable<EditorValidationIssue> Validate(SerializedProperty property, object obj);
    }

    public abstract class AttributeValidator<T> : FireValidator where T : IFireValidationAttribute
    {
        public virtual EditorValidationIssue Validate(SerializedProperty property, T attribute) => null;

        public virtual IEnumerable<EditorValidationIssue> ValidateMany(SerializedProperty property, T attribute) =>
            Enumerable.Empty<EditorValidationIssue>();

        public override IEnumerable<EditorValidationIssue> Validate(SerializedProperty property, object obj)
        {
            var issues = ValidateMany(property, (T)obj) ?? new List<EditorValidationIssue>();
            var issue = Validate(property, (T)obj);
            if (issue != null)
                issues = issues.Append(issue);
            return issues;
        }
    }
}