using System;
using JetBrains.Annotations;

namespace FireInspector.Attributes.Other
{
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowInInspectorAttribute : Attribute { }
}