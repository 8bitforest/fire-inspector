using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FireInspector.Editor.Validation
{
    public static class ValidatorProvider
    {
        private static readonly Dictionary<Type, object> Validators = new();

        static ValidatorProvider()
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

        public static Validator GetAttributeValidator(Type attributeType)
        {
            if (Validators.TryGetValue(attributeType, out var validator))
                return (Validator)validator;
            return null;
        }
    }
}