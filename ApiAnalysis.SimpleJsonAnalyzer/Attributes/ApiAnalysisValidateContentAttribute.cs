// <copyright file="ApiAnalysisValidateContentAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis;

// Note. This is not supported in SimpleJsonAnalyzer
[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisValidateContentAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisValidateContentAttribute(Type jsonType)
    {
        JsonType = jsonType;
    }

    public ApiAnalysisValidateContentAttribute(Type jsonType, string replaceThis, string withThis)
    {
        JsonType = jsonType;
        ReplaceThis = replaceThis;
        WithThis = withThis;
    }

    public Type JsonType { get; }

    public string ReplaceThis { get; }

    public string WithThis { get; }
}