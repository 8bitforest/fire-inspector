using FireInspector.Validation;
using UnityEngine;

namespace FireInspector.Attributes
{
    public abstract class ValidationAttribute : PropertyAttribute
    {
        public abstract IValidator Validator { get; }
    }
}