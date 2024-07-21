using FireInspector.Attributes.Validation;

namespace FireInspector.Attributes.Properties
{
    public class SelectAttribute : FireAttribute, IFireValidationAttribute
    {
        public string GetOptionsMethodName { get; private set; }
        public string DependsOn { get; private set; }

        public SelectAttribute(string getOptionsMethodName, string dependsOn = null)
        {
            GetOptionsMethodName = getOptionsMethodName;
            DependsOn = dependsOn;
        }
    }
}