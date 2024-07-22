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
        private SelectField _selectField;
        private Object _value;

        protected override VisualElement CreatePropertyElement(SerializedProperty property)
        {
            var options = SelectUtils.GetSelectOptions(property);
            _selectField = new SelectField(property.displayName, options);
            _selectField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            _selectField.RegisterValueChangedCallback(evt =>
            {
                property.boxedValue = evt.newValue?.Value;
                property.serializedObject.ApplyModifiedProperties();
            });
            _selectField.value = options.Find(o => o.Value.Equals(property.boxedValue));

            var selectAttribute = (SelectAttribute)attribute;
            if (!string.IsNullOrEmpty(selectAttribute.DependsOn))
            {
                OnOtherPropertyChanged(property, selectAttribute.DependsOn, () =>
                {
                    options = SelectUtils.GetSelectOptions(property);
                    _selectField.Options = options;
                    _selectField.SetValueWithoutNotify(options.Find(o => o.Value.Equals(property.boxedValue)));
                });
            }

            return _selectField;
        }

    }
}