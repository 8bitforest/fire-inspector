using System.Collections.Generic;
using System.Reflection;
using FireInspector.Attributes;
using FireInspector.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FireInspector.Validation
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
            foreach (var component in gameObject.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                ValidateComponent(issues, component);
            }
        }

        private static void ValidateComponent(List<ValidationIssue> issues, Component component)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = component.GetType().GetFields(flags);

            foreach (var field in fields)
            {
                var property = new InspectorProperty(component, field);
                var issuesForField = ValidateProperty(property);
                issues.AddRange(issuesForField);
            }
        }

        public static List<ValidationIssue> ValidateProperty(InspectorProperty property)
        {
            var issues = new List<ValidationIssue>();

            foreach (var attribute in property.GetAttributes<ValidationAttribute>())
            foreach (var issue in attribute.Validator.Validate(property))
                issues.Add(issue);

            return issues;
        }
    }
}