// <copyright file="ApiAnalysisConditionallyValidateContentAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis;

// Note. This is not supported in SimpleJsonAnalyzer
// The conditional check is based on another property of the object matching that defined
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class ApiAnalysisConditionallyValidateContentAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisConditionallyValidateContentAttribute(Type jsonType, string property, string value)
    {
        JsonType = jsonType;
        Property = property;
        Value = value;
    }

    public Type JsonType { get; }

    public string Property { get; }

    public string Value { get; }
}