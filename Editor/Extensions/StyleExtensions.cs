using UnityEngine.UIElements;

namespace FireInspector.Editor.Extensions
{
    public static class StyleExtensions
    {
        public static void SetMargin(this IStyle style, int value)
        {
            style.marginTop = value;
            style.marginRight = value;
            style.marginBottom = value;
            style.marginLeft = value;
        }
        
        public static void SetPadding(this IStyle style, int value)
        {
            style.paddingTop = value;
            style.paddingRight = value;
            style.paddingBottom = value;
            style.paddingLeft = value;
        }
        
        public static void SetBorderRadius(this IStyle style, int value)
        {
            style.borderTopLeftRadius = value;
            style.borderTopRightRadius = value;
            style.borderBottomLeftRadius = value;
            style.borderBottomRightRadius = value;
        }
        
        public static void SetBorderWidth(this IStyle style, int value)
        {
            style.borderTopWidth = value;
            style.borderRightWidth = value;
            style.borderBottomWidth = value;
            style.borderLeftWidth = value;
        }
    }
}