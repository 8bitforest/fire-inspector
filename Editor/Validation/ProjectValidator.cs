using System.Collections.Generic;
using System.Reflection;
using FireInspector.Attributes.Validation;
using FireInspector.Editor.Extensions;
using FireInspector.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FireInspector.Editor.Validation
{
    public static class ProjectValidator
    {
        public static List<ValidationIssue> ValidateProject()
        {
            var issues = new List<ValidationIssue>();

            // Scan all scenes in the project
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                ValidateScene(issues, scene);
            }

            // Scan all prefabs in the project
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                ValidateGameObject(issues, prefab);
            }

            return issues;
        }

        private static void ValidateScene(List<ValidationIssue> issues, Scene scene)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
                ValidateGameObject(issues, gameObject);
        }

        private static void ValidateGameObject(List<ValidationIssue> issues, GameObject gameObject)
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (component == null)
                {
                    issues.Add(ValidationIssue.Warning(gameObject, "GameObject contains a missing component!"));
                    continue;
                }

                ValidateComponent(issues, component);
            }

            foreach (Transform child in gameObject.transform)
                ValidateGameObject(issues, child.gameObject);
        }

        private static void ValidateComponent(List<ValidationIssue> issues, Component component)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = component.GetType().GetFields(flags);

            // If it's a builtin Unity component, skip validation
            if (component.GetType().Namespace?.StartsWith("UnityEngine") == true)
                return;

            foreach (var field in fields)
            {
                if (!IsSerialized(field))
                    continue;

                var property = new InspectorProperty(component, field);
                if (property.Property == null)
                    continue;

                var issuesForField = ValidateProperty(property);
                issues.AddRange(issuesForField);
            }
        }

        public static List<ValidationIssue> ValidateProperty(InspectorProperty property, bool validateChildren = true)
        {
            var issues = new List<ValidationIssue>();

            // If the property is a serialized class, validate its children instead
            if (property.Property.propertyType == SerializedPropertyType.Generic)
            {
                foreach (var attribute in property.GetAttributes<IFireValidationAttribute>())
                    issues.Add(ValidationIssue.NotSupported(property, attribute));

                if (!validateChildren)
                    return issues;

                var value = property.Property.GetGenericValue();
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                var fields = value.GetType().GetFields(flags);
                var serializedObject = property.Property.serializedObject;
                var path = property.Property.propertyPath;

                foreach (var field in fields)
                {
                    if (!IsSerialized(field))
                        continue;

                    var subPath = $"{path}.{field.Name}";
                    var subProperty = serializedObject.FindProperty(subPath);
                    var subInspectorProperty = new InspectorProperty(subProperty);
                    if (subInspectorProperty.Property == null)
                        continue;

                    issues.AddRange(ValidateProperty(subInspectorProperty));
                }
            }
            else
            {
                foreach (var attribute in property.GetAttributes<IFireValidationAttribute>())
                    issues.AddRange(ValidatePropertyAttribute(property, attribute));
            }

            return issues;
        }

        public static List<ValidationIssue> ValidatePropertyAttribute(InspectorProperty property,
            IFireValidationAttribute attribute)
        {
            var validator = FireAttributeTypes.GetAttributeValidator(attribute.GetType());
            if (validator == null)
            {
                return new List<ValidationIssue>
                {
                    ValidationIssue.Error(property, $"No validator found for attribute [{attribute.GetType().Name}]")
                };
            }

            var issues = validator.Validate(property, attribute);
            if (issues != null) return new List<ValidationIssue>(issues);
            return new List<ValidationIssue>();
        }

        public static List<ValidationIssue> ValidatePropertyReference(InspectorProperty property, string reference)
        {
            var issues = new List<ValidationIssue>();
            if (string.IsNullOrEmpty(reference))
                return issues;
            
            var referenceProperty = property.Property.FindSiblingProperty(reference);
            if (referenceProperty == null)
            {
                issues.Add(ValidationIssue.Error(property, $"Property '{reference}' not found."));
                return issues;
            }

            return issues;
        }

        private static bool IsSerialized(FieldInfo field)
        {
            if (field.IsStatic) return false;
            return field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;
        }
    }
}