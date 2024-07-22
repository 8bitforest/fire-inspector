using System;
using UnityEngine;

namespace FireInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FireAttribute : PropertyAttribute { }
}