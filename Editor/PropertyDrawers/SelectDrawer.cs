using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Properties;
using FireInspector.Extensions;
using UnityEditor.UIElements;

namespace FireInspector.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SelectAttribute))]
    public class SelectDrawer : FirePropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var obj = property.GetContainingObject();

            var methodName = (attribute as SelectAttribute)!.GetListMethodName;
            var method = obj!.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var items = method!.Invoke(obj, null) as IEnumerable<object>;

            // Create a list of strings for the dropdown
            var itemList = items?.Select(item => item.ToString()).ToList() ?? new List<string>();

            // Create a PopupField (dropdown) with the items
            var popupField = new PopupField<string>(property.displayName, itemList, 0);
            popupField.RegisterValueChangedCallback(evt =>
            {
                property.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            // Set the initial value of the dropdown
            popupField.value = property.stringValue;

            popupField.AddToClassList(BaseField<ObjectField>.ussClassName);
            popupField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            popupField.labelElement.AddToClassList(PropertyField.labelUssClassName);

            return popupField;
        }
    }
}