using System;
using FireInspector.Attributes.Properties;

namespace FireInspector.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class RequiredAttribute : FireAttribute, IFireValidationAttribute { }
}