using System.Collections.Generic;
using System.Linq;
using FireInspector.Attributes.Properties;
using FireInspector.Editor.Elements;
using FireInspector.Elements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features.Drawers
{
    [CustomPropertyDrawer(typeof(ComponentSelectAttribute))]
    public class ComponentSelectDrawer : FirePropertyDrawer
    {
        protected override VisualElement CreatePropertyElement(SerializedProperty property)
        {
            var pickerAttribute = (ComponentSelectAttribute)attribute;
            var options = new List<SelectOption<Object>>();
            var targetField = new ObjectField(pickerAttribute.TargetDisplayName);
            var componentField = new SelectField<Object>(property.displayName, options);

            componentField.BindProperty(property);
            componentField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            componentField.RegisterValueChangedCallback(e =>
            {
                targetField.SetValueWithoutNotify((e.newValue as Component)?.gameObject);
                UpdateOptions(targetField, componentField);
            });

            targetField.value = (componentField.value as Component)?.gameObject;
            targetField.objectType = typeof(GameObject);
            targetField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            targetField.RegisterValueChangedCallback(_ =>
            {
                UpdateOptions(targetField, componentField);
                componentField.value = null;
            });

            var container = new VisualElement();
            container.Add(targetField);
            container.Add(componentField);
            return container;
        }

        private void UpdateOptions(ObjectField targetField, SelectField<Object> componentField)
        {
            componentField.Options = targetField.value == null
                ? new List<SelectOption<Object>>()
                : GetOptions(targetField.value as GameObject);
        }

        private List<SelectOption<Object>> GetOptions(GameObject target)
        {
            if (target == null)
                return new List<SelectOption<Object>>();

            Texture GetComponentImage(Component c)
            {
#if UNITY_EDITOR
                return EditorGUIUtility.ObjectContent(c, c.GetType()).image;
#else
                return null;
#endif
            }

            return new List<SelectOption<Object>>(target.GetComponents<Component>()
                .Where(c => c != null)
                .Select(c =>
                    new SelectOption<Object>(
                        c.GetType().Name,
                        c,
                        GetComponentImage(c)
                    )));
        }
    }
}