using FireInspector.Attributes.Properties;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features.Drawers
{
    [CustomPropertyDrawer(typeof(SelectAttribute))]
    public class SelectDrawer : FirePropertyDrawer
    {
        protected override VisualElement CreatePropertyElement(SerializedProperty property)
        {
            var type = property.propertyType;
            switch (type)
            {
                case SerializedPropertyType.Integer:
                    return CreateSelectField<int>(property);
                case SerializedPropertyType.Float:
                    return CreateSelectField<float>(property);
                case SerializedPropertyType.String:
                    return CreateSelectField<string>(property);
                case SerializedPropertyType.ObjectReference:
                    return CreateSelectField<Object>(property);
                default:
                    var field = new TextField(property.displayName);
                    field.value = "Unsupported type";
                    field.isReadOnly = true;
                    field.SetEnabled(false);
                    field.AddToClassList(TextField.alignedFieldUssClassName);
                    return field;
            }
        }

        private VisualElement CreateSelectField<T>(SerializedProperty property)
        {
            var options = SelectUtils.GetSelectOptions<T>(property);
            var selectField = new SelectField<T>(property.displayName, options);
            selectField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            selectField.BindProperty(property);

            var selectAttribute = (SelectAttribute)attribute;
            if (!string.IsNullOrEmpty(selectAttribute.DependsOn))
            {
                OnOtherPropertyChanged(property, selectAttribute.DependsOn, () =>
                {
                    options = SelectUtils.GetSelectOptions<T>(property);
                    selectField.Options = options;
                    selectField.SetValueWithoutNotify((T)property.boxedValue);
                });
            }

            return selectField;
        }
    }
}