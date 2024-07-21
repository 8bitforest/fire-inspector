namespace FireInspector.Attributes.Properties
{
    public class ComponentSelectAttribute : FireAttribute
    {
        public string TargetDisplayName { get; private set; }

        public ComponentSelectAttribute(string targetDisplayName = "Target")
        {
            TargetDisplayName = targetDisplayName;
        }
    }
}