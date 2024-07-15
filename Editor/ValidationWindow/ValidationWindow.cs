using System.Collections.Generic;
using FireInspector.Validation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.ValidationWindow
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
                "Packages/com.eightbitforest.fire-inspector/Editor/ValidationWindow/ValidationWindow.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.eightbitforest.fire-inspector/Editor/ValidationWindow/ValidationWindow.uss");
            _issueTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.eightbitforest.fire-inspector/Editor/ValidationWindow/ValidationWindowIssue.uxml");

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
                    issueElement.Q<Label>("issueProperty").text =
                        $"{issue.Property.ComponentName}.{issue.Property.Name}";
                    issueElement.Q<Label>("issuePath").text = issue.Property.GetObjectPath();
                    issueElement.Q<Label>("issueMessage").text = issue.Message;
                    _issuesScrollView.Add(issueElement);
                }
            }

            Debug.Log("Scan complete.");
            Debug.Log($"Found {_issues.Count} issues.");
        }
    }
}