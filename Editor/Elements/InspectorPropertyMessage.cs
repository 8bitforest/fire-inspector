using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FireInspector.Editor.Elements
{
    public class InspectorPropertyMessage : VisualElement
    {
        public enum MessageType { Success, Info, Warning, Error }

        private MessageType _messageType;
        private readonly Label _label;
        private readonly Image _icon;
        private readonly VisualElement _content;
        public override VisualElement contentContainer => _content;

        public string Text
        {
            get => _label.text;
            set
            {
                _label.text = value;
                _label.style.display = string.IsNullOrEmpty(value) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        public MessageType Type
        {
            get => _messageType;
            set
            {
                _messageType = value;
                var borderColor = _messageType switch
                {
                    MessageType.Success => new Color(0.2f, 1f, .6f),
                    MessageType.Info => new Color(0.2f, 0.6f, 1f),
                    MessageType.Warning => new Color(1f, 0.8f, 0.2f),
                    MessageType.Error => new Color(1f, 0.2f, 0.2f),
                    _ => Color.white
                };

                var backgroundColor = _messageType switch
                {
                    MessageType.Success => new Color(0.9f, 1, 0.9f),
                    MessageType.Info => new Color(0.9f, 0.9f, 1),
                    MessageType.Warning => new Color(1, 1, 0.9f),
                    MessageType.Error => new Color(1, 0.9f, 0.9f),
                    _ => Color.white
                };

                style.borderTopColor = borderColor;
                style.borderRightColor = borderColor;
                style.borderBottomColor = borderColor;
                style.borderLeftColor = borderColor;
                style.backgroundColor = backgroundColor;

                _icon.image = _messageType switch
                {
                    MessageType.Success => EditorGUIUtility.IconContent("TestPassed").image,
                    MessageType.Info => EditorGUIUtility.IconContent("console.infoicon").image,
                    MessageType.Warning => EditorGUIUtility.IconContent("console.warnicon").image,
                    MessageType.Error => EditorGUIUtility.IconContent("console.erroricon").image,
                    _ => EditorGUIUtility.IconContent("console.infoicon").image
                };

                _label.style.color = _messageType switch
                {
                    MessageType.Error => Color.red,
                    _ => Color.black
                };
            }
        }

        public InspectorPropertyMessage() : this(string.Empty, MessageType.Info) { }

        public InspectorPropertyMessage(string message, MessageType messageType)
        {
            _content = new VisualElement();
            _content.style.flexDirection = FlexDirection.Column;

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;

            style.borderTopWidth = 1;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderLeftWidth = 1;
            style.borderTopLeftRadius = 5;
            style.borderTopRightRadius = 5;
            style.borderBottomLeftRadius = 5;
            style.borderBottomRightRadius = 5;
            style.paddingTop = 5;
            style.paddingRight = 5;
            style.paddingBottom = 5;
            style.paddingLeft = 5;
            style.marginTop = 5;
            style.marginBottom = 5;

            _icon = new Image();
            _icon.style.width = 20;
            _icon.style.height = 20;
            _icon.style.marginRight = 5;
            hierarchy.Add(_icon);
            hierarchy.Add(_content);

            _label = new Label(message);
            Add(_label);

            Text = message;
            Type = messageType;
        }

        public new class UxmlFactory : UxmlFactory<InspectorPropertyMessage, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlEnumAttributeDescription<MessageType> _messageType = new()
                { name = "message-type", defaultValue = MessageType.Info };

            private readonly UxmlStringAttributeDescription _message = new() { name = "message", defaultValue = "" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var element = (ve as InspectorPropertyMessage)!;
                var message = _message.GetValueFromBag(bag, cc);
                var messageType = _messageType.GetValueFromBag(bag, cc);
                element.Text = message;
                element.Type = messageType;
            }
        }
    }
}