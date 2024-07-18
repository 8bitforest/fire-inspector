using System;
using FireInspector.Validation;
using FireInspector.Validation.Validators;

namespace FireInspector.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class RequiredAttribute : FireValidationAttribute
    {
        public override IValidator Validator => new RequiredValidator();
    }
}