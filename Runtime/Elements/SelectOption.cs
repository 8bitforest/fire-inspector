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
        public new T Value { get; }

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

        protected bool Equals(SelectOption<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SelectOption<T>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public static implicit operator SelectOption<T>(T value) => new(value);
    }

    public class SelectOptionList<T> : List<SelectOption<T>>
    {
        public SelectOptionList() { }

        public SelectOptionList(IEnumerable<SelectOption<T>> options) : base(options) { }
    }
}