using System;
using UnityEngine;

namespace FireInspector.Attributes.Properties
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FirePropertyAttribute : PropertyAttribute { }
}