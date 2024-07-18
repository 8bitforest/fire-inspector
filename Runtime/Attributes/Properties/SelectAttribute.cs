namespace FireInspector.Attributes.Properties
{
    public class SelectAttribute : FirePropertyAttribute
    {
        public string GetListMethodName { get; private set; }

        public SelectAttribute(string getListMethodName)
        {
            GetListMethodName = getListMethodName;
        }
    }
}