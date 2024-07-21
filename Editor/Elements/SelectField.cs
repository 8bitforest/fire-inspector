using System;
using System.Collections.Generic;
using FireInspector.Editor.Extensions;
using FireInspector.Elements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using BasePopupField = UnityEngine.UIElements.BasePopupField<string, string>;

namespace FireInspector.Editor.Elements
{
    public class SelectField : BaseField<SelectOption>
    {
        private VisualElement VisualInput => this.Q<VisualElement>(className: inputUssClassName);
        
        private readonly List<SelectOption> _options;

        public List<SelectOption> Options
        {
            get => _options;
            set
            {
                _options.Clear();
                _options.AddRange(value);
                UpdateLabel();
            }
        }

        private bool _popupVisible;
        private SelectPopupWindow _popup;
        private readonly TextElement _textElement;
        private readonly Image _iconElement;

        public SelectField(string label, List<SelectOption> options) : base(label, new VisualElement())
        {
            _options = options;

            AddToClassList(BasePopupField.ussClassName);
            VisualInput.AddToClassList(BasePopupField.inputUssClassName);
            VisualInput.style.alignItems = Align.Center;

            _iconElement = new Image();
            _iconElement.style.width = 14;
            _iconElement.style.height = 12;
            _iconElement.style.marginRight = 2;
            VisualInput.Add(_iconElement);

            _textElement = new SelectTextElement();
            _textElement.pickingMode = PickingMode.Ignore;
            _textElement.AddToClassList(BasePopupField.textUssClassName);
            VisualInput.Add(_textElement);

            var arrowElement = new VisualElement();
            arrowElement.AddToClassList(BasePopupField.arrowUssClassName);
            arrowElement.pickingMode = PickingMode.Ignore;
            VisualInput.Add(arrowElement);

            RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            RegisterCallback<ChangeEvent<SelectOption>>(_ => { UpdateLabel(); });
        }

        public override void SetValueWithoutNotify(SelectOption newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdateLabel();
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                schedule.Execute(TogglePopup);
                evt.StopPropagation();
            }
        }

        private void TogglePopup()
        {
            try
            {
                var closed = _popup == null || _popup.Equals(null);
                if (closed)
                {
                    _popup = ScriptableObject.CreateInstance<SelectPopupWindow>();
                    _popup.Initialize(_options, new List<SelectOption> { value },
                        options =>
                        {
                            value = options[0]; 
                            _popup.Close();
                            _popup = null;
                        });
                    var rect = GUIUtility.GUIToScreenRect(VisualInput.worldBound);
                    var width = VisualInput.worldBound.width;
                    _popup.ShowAsDropDown(rect, new Vector2(width, 300));
                }
                else
                {
                    _popup.Close();
                    _popup = null;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void UpdateLabel()
        {
            var valid = _options.Contains(value);
            if (valid && value?.Icon != null)
            {
                _iconElement.style.display = DisplayStyle.Flex;
                _iconElement.image = value.Icon;
            }
            else
            {
                _iconElement.style.display = DisplayStyle.None;
            }

            _textElement.text = valid ? value?.Text : "";
        }

        private class SelectTextElement : TextElement { }

        private class SelectPopupWindow : EditorWindow
        {
            private List<SelectOption> _allOptions;
            private List<SelectOption> _filteredOptions;
            private List<SelectOption> _selectedOptions;
            private Action<List<SelectOption>> _onSelectionChanged;

            private ListView _listView;
            private SearchFieldBase<TextField, string> _searchField;

            public void Initialize(List<SelectOption> options, List<SelectOption> selectedOptions,
                Action<List<SelectOption>> onSelectionChanged)
            {
                _allOptions = options;
                _filteredOptions = options;
                _selectedOptions = selectedOptions;
                _onSelectionChanged = onSelectionChanged;
            }

            private void CreateGUI()
            {
                var root = rootVisualElement;
                root.style.flexDirection = FlexDirection.Column;

                _searchField = new ToolbarSearchField();
                _searchField.RegisterValueChangedCallback(OnSearchValueChanged);
                _searchField.style.SetMargin(5);
                _searchField.style.width = new StyleLength(StyleKeyword.Auto); // Ensure it takes full width
                root.Add(_searchField);

                var backgroundColor = new Color(0.87f, 0.87f, 0.87f);
                if (EditorGUIUtility.isProSkin)
                    backgroundColor = new Color(0.31f, 0.31f, 0.31f);

                var header = new Label("Options");
                header.style.unityFontStyleAndWeight = FontStyle.Bold;
                header.style.unityTextAlign = TextAnchor.MiddleCenter;
                header.style.backgroundColor = backgroundColor;
                header.style.SetPadding(5);
                root.Add(header);

                _listView = new ListView(_allOptions, -1, () => new OptionElement(), (element, index) =>
                {
                    var optionElement = (OptionElement)element;
                    var selectOption = _listView.itemsSource[index] as SelectOption;
                    var selected = _selectedOptions.Contains(selectOption);
                    optionElement.Bind(selectOption, selected, option =>
                    {
                        _selectedOptions = new List<SelectOption> { option };
                        _listView.itemsSource = new List<SelectOption>(_filteredOptions);
                        _onSelectionChanged?.Invoke(_selectedOptions);
                    });
                });
                _listView.style.flexGrow = 1;
                root.Add(_listView);

                _searchField.Focus();
                _searchField.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Escape)
                    {
                        Close();
                        evt.StopPropagation();
                    }
                });
                _searchField.RegisterCallback<FocusOutEvent>(evt =>
                {
                    evt.StopPropagation();
                    root.schedule.Execute(() => _searchField.Focus());
                });
            }

            private void OnSearchValueChanged(ChangeEvent<string> evt)
            {
                var search = evt.newValue;
                _filteredOptions = _allOptions.FindAll(option => option.Text.ToLower().Contains(search.ToLower()));
                _listView.itemsSource = _filteredOptions;
            }

            private class OptionElement : VisualElement
            {
                private SelectOption _option;
                private Action<SelectOption> _onOptionSelected;

                private readonly Label _label;
                private readonly Image _icon;
                private readonly Image _checkmark;

                public OptionElement()
                {
                    style.flexDirection = FlexDirection.Row;
                    style.alignItems = Align.Center;
                    style.paddingLeft = 5;

                    _checkmark = new Image();
                    _checkmark.image = EditorGUIUtility.IconContent("Checkmark").image;
                    _checkmark.style.width = 8;
                    _checkmark.style.height = 8;
                    _checkmark.style.marginRight = 5;

                    _icon = new Image();
                    _icon.style.width = 16;
                    _icon.style.height = 16;
                    _icon.style.marginRight = 5;

                    _label = new Label();

                    Add(_checkmark);
                    Add(_icon);
                    Add(_label);
                }

                public void Bind(SelectOption option, bool selected, Action<SelectOption> onOptionSelected)
                {
                    _option = option;
                    _onOptionSelected = onOptionSelected;

                    _label.text = option.Text;
                    _checkmark.visible = selected;

                    if (option.Icon != null)
                    {
                        _icon.style.display = DisplayStyle.Flex;
                        _icon.image = option.Icon;
                    }
                    else
                    {
                        _icon.style.display = DisplayStyle.None;
                    }

                    RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
                }

                private void OnPointerDownEvent(PointerDownEvent evt)
                {
                    if (evt.button == (int)MouseButton.LeftMouse)
                    {
                        _checkmark.visible = !_checkmark.visible;
                        _onOptionSelected?.Invoke(_option);
                        evt.StopPropagation();
                    }
                }
            }
        }
    }
}