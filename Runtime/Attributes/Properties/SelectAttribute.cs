using FireInspector.Attributes.Validation;

namespace FireInspector.Attributes.Properties
{
    public class SelectAttribute : FireAttribute, IFireValidationAttribute
    {
        public string GetOptionsMethodName { get; private set; }

        public SelectAttribute(string getOptionsMethodName)
        {
            GetOptionsMethodName = getOptionsMethodName;
        }
    }
}