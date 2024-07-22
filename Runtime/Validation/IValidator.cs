using System.Collections.Generic;
using System.Linq;

namespace FireInspector.Validation
{
    public interface IFireValidator
    {
        public ValidationIssue Validate() => null;
        public IEnumerable<ValidationIssue> ValidateMany() => Enumerable.Empty<ValidationIssue>();
    }
}