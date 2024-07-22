using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Utils
{
    public static class ShowInInspectorDrawer
    {
        private static readonly Func<Type, bool, Type> GetDrawerTypeForTypeMethod;
        private static readonly FieldInfo DecoratorDrawerAttributeField;

        static ShowInInspectorDrawer()
        {
            var scriptAttributeUtility = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor");
            if (scriptAttributeUtility == null)
            {
                Debug.LogError("Failed to find UnityEditor.ScriptAttributeUtility");
                return;
            }

            var getDrawerTypeForTypeMethod = scriptAttributeUtility.GetMethod("GetDrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (getDrawerTypeForTypeMethod == null)
            {
                Debug.LogError("Failed to find UnityEditor.ScriptAttributeUtility.GetDrawerTypeForType");
                return;
            }

            GetDrawerTypeForTypeMethod = (Func<Type, bool, Type>)Delegate.CreateDelegate(typeof(Func<Type, bool, Type>),
                getDrawerTypeForTypeMethod);

            DecoratorDrawerAttributeField =
                typeof(DecoratorDrawer).GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static VisualElement CreateFieldGUI(SerializedObject serializedObject, FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
            var drawersContainer = new VisualElement();
            drawersContainer.AddToClassList("unity-decorator-drawers-container");

            // Add any decorator drawers to the property
            foreach (var attr in attributes)
            {
                var decoratorDrawer = FindDecoratorDrawer((PropertyAttribute)attr);
                if (decoratorDrawer != null)
                {
                    var element = decoratorDrawer.CreatePropertyGUI();
                    drawersContainer.Add(element);
                }
            }

            var labelField = new PropertyField();
            labelField.Add(CreateFieldElement(serializedObject, field));

            var property = new PropertyField();
            property.Add(drawersContainer);
            property.Add(labelField);

            return property;
        }

        private static DecoratorDrawer FindDecoratorDrawer(PropertyAttribute attribute)
        {
            var drawerType = GetDrawerTypeForType(attribute.GetType());
            if (typeof(DecoratorDrawer).IsAssignableFrom(drawerType))
            {
                var drawer = (DecoratorDrawer)Activator.CreateInstance(drawerType);
                DecoratorDrawerAttributeField.SetValue(drawer, attribute);
                return drawer;
            }

            return null;
        }

        private static Type GetDrawerTypeForType(Type type, bool isPropertyTypeAManagedReference = false)
        {
            return GetDrawerTypeForTypeMethod(type, isPropertyTypeAManagedReference);
        }

        private static VisualElement CreateFieldElement(SerializedObject serializedObject, FieldInfo field)
        {
            var value = field.GetValue(serializedObject.targetObject);
            var element = new TextElement();
            element.enableRichText = true;
            element.text = value?.ToString() ?? string.Empty;
            return element;
        }
    }
}