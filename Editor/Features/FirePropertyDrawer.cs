using System;
using FireInspector.Editor.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Features
{
    public abstract class FirePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return CreatePropertyElement(property);
        }

        protected abstract VisualElement CreatePropertyElement(SerializedProperty property);

        protected void OnOtherPropertyChanged(SerializedProperty property, string otherPropertyName, Action onChanged)
        {
            var otherProperty = property.FindSiblingProperty(otherPropertyName);
            if (otherProperty == null)
                return;

            var editorProperty = FireEditor.FindEditorProperty(otherProperty);
            if (editorProperty == null)
                return;

            editorProperty.OnChanged(onChanged);
        }
    }
}