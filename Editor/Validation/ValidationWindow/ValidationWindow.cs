using System.Collections.Generic;
using FireInspector.Editor.Elements;
using FireInspector.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Validation.ValidationWindow
{
    public class FirePropertyValidationWindow : EditorWindow
    {
        private List<ValidationIssue> _issues = new();
        private ScrollView _issuesScrollView;
        private VisualElement _startupElement;
        private VisualElement _noIssuesElement;
        private VisualTreeAsset _issueTemplate;

        [MenuItem("Window/Fire Inspector/Project Validator")]
        public static void ShowWindow()
        {
            FirePropertyValidationWindow window = GetWindow<FirePropertyValidationWindow>();
            window.titleContent = new GUIContent("Project Validator");
            window.Show();
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.eightbitforest.fire-inspector/Editor/Validation/ValidationWindow/ValidationWindow.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.eightbitforest.fire-inspector/Editor/Validation/ValidationWindow/ValidationWindow.uss");
            _issueTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.eightbitforest.fire-inspector/Editor/Validation/ValidationWindow/ValidationWindowIssue.uxml");

            VisualElement root = visualTree.CloneTree();
            root.styleSheets.Add(styleSheet);
            rootVisualElement.Add(root);

            var rescanButton = root.Q<Button>("validateButton");
            _issuesScrollView = root.Q<ScrollView>("issues");
            _noIssuesElement = root.Q<VisualElement>("noIssues");
            _startupElement = root.Q<VisualElement>("startup");

            _startupElement.style.display = DisplayStyle.Flex;
            _noIssuesElement.style.display = DisplayStyle.None;

            rescanButton.clicked += ScanProject;
            ScanProject();
        }

        private void ScanProject()
        {
            Debug.Log("Scanning project...");

            _issues = ProjectValidator.ValidateProject();

            _issuesScrollView.Clear();
            _startupElement.style.display = DisplayStyle.None;

            if (_issues.Count == 0)
            {
                _noIssuesElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                _noIssuesElement.style.display = DisplayStyle.None;
                _issuesScrollView.Clear();
                foreach (var issue in _issues)
                {
                    var issueElement = _issueTemplate.Instantiate();

                    issueElement.Q<Label>("issueHeader").text = issue.GameObject.GetObjectPath();
                    issueElement.Q<Label>("issueSubHeader").text = issue.Property != null
                        ? $"{issue.Property?.Component.GetType().Name} > {issue.Property?.Name}"
                        : "(Script)";
                    issueElement.Q<Label>("issueMessage").text = issue.Message;
                    issueElement.Q<InspectorPropertyMessage>("message").Type = issue.IssueSeverity switch
                    {
                        ValidationIssue.Severity.Info => InspectorPropertyMessage.MessageType.Info,
                        ValidationIssue.Severity.Warning => InspectorPropertyMessage.MessageType.Warning,
                        ValidationIssue.Severity.Error => InspectorPropertyMessage.MessageType.Error,
                        _ => InspectorPropertyMessage.MessageType.Info
                    };
                    _issuesScrollView.Add(issueElement);
                }
            }

            Debug.Log("Scan complete.");
            Debug.Log($"Found {_issues.Count} issues.");
        }
    }
}