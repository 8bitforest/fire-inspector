using System.Collections.Generic;
using FireInspector.Utils;

namespace FireInspector.Validation
{
    public interface IValidator
    {
        public IEnumerable<ValidationIssue> Validate(InspectorProperty property);
    }
}