using System;
using UnityEditor;
using UnityEngine.UIElements;
using FireInspector.Attributes.Properties;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Utils;
using UnityEditor.UIElements;

namespace FireInspector.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SelectAttribute))]
    public class SelectDrawer : FirePropertyDrawer
    {
        protected override VisualElement CreateFieldElement(SerializedProperty property, Action onChange)
        {
            var options = SelectUtils.GetSelectOptions(property);
            var popupField = new SelectField(property.displayName, options);
            popupField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            popupField.RegisterValueChangedCallback(evt =>
            {
                property.boxedValue = evt.newValue.Value;
                property.serializedObject.ApplyModifiedProperties();
                onChange();
            });
            
            var option = options.Find(o => o.Value.Equals(property.boxedValue));
            if (option != null)
                popupField.value = option;

            return popupField;
        }
    }
}