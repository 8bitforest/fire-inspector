using System;
using System.Collections.Generic;
using System.Reflection;
using FireInspector.Editor.Features;
using UnityEngine;

namespace FireInspector.Editor.Utils
{
    public static class FireAttributeTypes
    {
        private static readonly Dictionary<Type, object> Validators = new();

        static FireAttributeTypes()
        {
            var startTime = Time.realtimeSinceStartup;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                FindValidatorsInAssembly(assembly);

            Debug.Log($"Found {Validators.Count} validators in {Time.realtimeSinceStartup - startTime} seconds.");
        }

        private static void FindValidatorsInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                if (type.IsGenericType) continue;

                if (type.BaseType is { IsGenericType: true } &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(AttributeValidator<>))
                {
                    var attributeType = type.BaseType.GetGenericArguments()[0];
                    Validators.Add(attributeType, Activator.CreateInstance(type));
                }
            }
        }

        public static FireValidator GetAttributeValidator(Type attributeType)
        {
            if (Validators.TryGetValue(attributeType, out var validator))
                return (FireValidator)validator;
            return null;
        }
    }
}