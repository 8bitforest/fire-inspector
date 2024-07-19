using System.Collections.Generic;
using UnityEngine;

namespace FireInspector.Elements
{
    public abstract class SelectOption
    {
        public string Text { get; protected set; }
        public object Value { get; protected set; }
        public Texture Icon { get; protected set; }
    }

    public class SelectOption<T> : SelectOption
    {
        public new T Value { get; private set; }

        public SelectOption(T value)
        {
            Value = value;
            base.Value = value;
            Text = value.ToString();
        }

        public SelectOption(string text, T value)
        {
            Text = text;
            Value = value;
            base.Value = value;
        }

        public SelectOption(string text, T value, Texture icon)
        {
            Text = text;
            Value = value;
            base.Value = value;
            Icon = icon;
        }

        public static implicit operator SelectOption<T>(T value) => new(value);
    }

    public class SelectOptionList<T> : List<SelectOption<T>>
    {
        public SelectOptionList() { }

        public SelectOptionList(IEnumerable<SelectOption<T>> options) : base(options) { }
    }
}