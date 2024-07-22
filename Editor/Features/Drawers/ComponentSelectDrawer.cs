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
            var options = new List<SelectOption>();
            var targetField = new ObjectField(pickerAttribute.TargetDisplayName);
            var componentField = new SelectField(property.displayName, options);

            targetField.objectType = typeof(GameObject);
            targetField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            targetField.RegisterValueChangedCallback(_ =>
            {
                UpdateOptions(targetField, componentField);
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                componentField.value = null;
            });

            componentField.AddToClassList(BaseField<ObjectField>.alignedFieldUssClassName);
            componentField.RegisterValueChangedCallback(evt =>
            {
                property.objectReferenceValue = evt.newValue?.Value as Component;
                property.serializedObject.ApplyModifiedProperties();
            });

            var value = property.objectReferenceValue;
            if (value != null)
            {
                targetField.value = (value as Component)?.gameObject;
                UpdateOptions(targetField, componentField);
                var option = options.Find(o => o.Value.Equals(value));
                componentField.value = option;
            }

            var container = new VisualElement();
            container.Add(targetField);
            container.Add(componentField);
            return container;
        }

        private void UpdateOptions(ObjectField targetField, SelectField componentField)
        {
            componentField.Options = targetField.value == null
                ? new List<SelectOption>()
                : GetOptions(targetField.value as GameObject).ToList<SelectOption>();
        }

        private SelectOptionList<Component> GetOptions(GameObject target)
        {
            if (target == null)
                return new SelectOptionList<Component>();

            Texture GetComponentImage(Component c)
            {
#if UNITY_EDITOR
                return EditorGUIUtility.ObjectContent(c, c.GetType()).image;
#else
                return null;
#endif
            }

            return new SelectOptionList<Component>(target.GetComponents<Component>()
                .Where(c => c != null)
                .Select(c =>
                    new SelectOption<Component>(
                        c.GetType().Name,
                        c,
                        GetComponentImage(c)
                    )));
        }
    }
}