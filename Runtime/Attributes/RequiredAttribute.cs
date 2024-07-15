using System;
using FireInspector.Validation;
using FireInspector.Validation.Validators;

namespace FireInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : ValidationAttribute
    {
        public override IValidator Validator => new RequiredValidator();
    }
}