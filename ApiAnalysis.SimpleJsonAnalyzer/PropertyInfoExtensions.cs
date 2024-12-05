// <copyright file="PropertyInfoExtensions.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ApiAnalysis
{
    public static class PropertyInfoExtensions
    {
        internal static string JsonPropertyName(this PropertyInfo pi)
        {
            foreach (var customAttributeData in pi.CustomAttributes)
            {
                if (customAttributeData.AttributeType?.Name != nameof(JsonPropertyAttribute))
                    continue;

                if (customAttributeData.ConstructorArguments.Any())
                    return customAttributeData.ConstructorArguments[0].Value.ToString();

                if (customAttributeData.NamedArguments == null)
                    continue;

                foreach (var namedArgument in customAttributeData.NamedArguments)
                {
                    if (namedArgument.MemberName == nameof(JsonPropertyAttribute.PropertyName))
                        return namedArgument.TypedValue.Value.ToString();
                }
            }

            return null;
        }

        internal static string DataMemberName(this PropertyInfo pi)
        {
            foreach (var customAttributeData in pi.CustomAttributes)
            {
                if (customAttributeData.AttributeType?.Name == nameof(DataMemberAttribute))
                {
                    if (customAttributeData.NamedArguments != null)
                    {
                        foreach (var namedArgument in customAttributeData.NamedArguments)
                        {
                            if (namedArgument.MemberName == nameof(DataMemberAttribute.Name))
                            {
                                return namedArgument.TypedValue.Value.ToString();
                            }
                        }
                    }
                }
            }

            return null;
        }

        internal static bool HasCustomAttribute(this PropertyInfo pi, Type attribute)
        {
            return GetCustomAttributeData(pi, attribute) != null;
        }

        internal static bool HasCustomAttributeAndProperty(this PropertyInfo pi, string attributeName, string namedArgumentName, string value)
        {
            // This assumes that there is only a single constructor argument or named property which may not always be the case
            return pi.CustomAttributes
                .Any(customAttributeData => customAttributeData.AttributeType?.Name == attributeName
                                            && ((customAttributeData.ConstructorArguments.Any()
                                              && (customAttributeData.ConstructorArguments[0].Value?.ToString() == value))
                                             || ((customAttributeData.NamedArguments != null)
                                                && customAttributeData.NamedArguments.Any(arg => arg.MemberName == namedArgumentName)
                                                && customAttributeData.NamedArguments[0].TypedValue.Value.ToString() == value)));
        }

        private static CustomAttributeData GetCustomAttributeData(this PropertyInfo pi, Type attribute)
        {
            return pi.CustomAttributes
                     .FirstOrDefault(ca => ca.AttributeType?.Name == attribute.Name
                                        || ca.AttributeType?.BaseType?.Name == attribute.Name);
        }
    }
}
