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

            // Scan all scriptable objects in the project
            var scriptableObjectGuids = AssetDatabase.FindAssets("t:ScriptableObject");
            foreach (var guid in scriptableObjectGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                ValidateScriptableObject(issues, scriptableObject);
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
            // If it's a builtin Unity component, skip validation
            if (component.GetType().Namespace?.StartsWith("UnityEngine") == true)
                return;

            ValidateSerializableObject(issues, new SerializedObject(component));
        }

        private static void ValidateScriptableObject(List<ValidationIssue> issues, ScriptableObject scriptableObject)
        {
            ValidateSerializableObject(issues, new SerializedObject(scriptableObject));
        }

        private static void ValidateSerializableObject(List<ValidationIssue> issues, SerializedObject serializedObject)
        {
            var property = serializedObject.GetIterator();
            var enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                if (property.isArray)
                    enterChildren = false;
                else
                    enterChildren = true;

                var issuesForField = ValidateProperty(property);
                issues.AddRange(issuesForField);
            }
        }

        public static List<ValidationIssue> ValidateProperty(SerializedProperty property)
        {
            var issues = new List<ValidationIssue>();
            if (!property.IsValid())
                return issues;

            var field = property.GetFieldInfo();
            if (field == null)
                return new List<ValidationIssue>();

            var attributes = field.GetCustomAttributes(typeof(IFireValidationAttribute), true)
                as IFireValidationAttribute[];
            if (attributes == null || attributes.Length == 0)
                return issues;

            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                foreach (var attribute in property.GetAttributes<IFireValidationAttribute>())
                    issues.AddRange(ValidatePropertyAttribute(property, attribute));
            }
            else if (property.propertyType == SerializedPropertyType.Generic)
            {
                foreach (var attribute in attributes)
                    issues.Add(ValidationIssue.NotSupported(property, attribute));
            }
            else
            {
                foreach (var attribute in attributes)
                    issues.AddRange(ValidatePropertyAttribute(property, attribute));
            }

            return issues;
        }

        public static List<ValidationIssue> ValidatePropertyAttribute(SerializedProperty property,
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

        public static List<ValidationIssue> ValidatePropertyReference(SerializedProperty property, string reference)
        {
            var issues = new List<ValidationIssue>();
            if (string.IsNullOrEmpty(reference))
                return issues;

            var referenceProperty = property.FindSiblingProperty(reference);
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