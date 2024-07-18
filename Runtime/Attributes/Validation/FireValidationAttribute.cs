using FireInspector.Validation;
using UnityEngine;

namespace FireInspector.Attributes.Validation
{
    public abstract class FireValidationAttribute : PropertyAttribute
    {
        public abstract IValidator Validator { get; }
    }
}