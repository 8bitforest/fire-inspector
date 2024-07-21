using System;
using FireInspector.Attributes.Properties;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features.Drawers
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
                property.boxedValue = evt.newValue?.Value;
                property.serializedObject.ApplyModifiedProperties();
                onChange();
            });
            popupField.value = options.Find(o => o.Value.Equals(property.boxedValue));

            var selectAttribute = (SelectAttribute)attribute;
            if (!string.IsNullOrEmpty(selectAttribute.DependsOn))
            {
                var dependsOnProperty = FindProperty(selectAttribute.DependsOn);
                dependsOnProperty?.OnChanged(() =>
                {
                    options = SelectUtils.GetSelectOptions(property);
                    popupField.Options = options;
                    popupField.SetValueWithoutNotify(options.Find(o => o.Value.Equals(property.boxedValue)));
                    onChange();
                });
            }

            return popupField;
        }
    }
}