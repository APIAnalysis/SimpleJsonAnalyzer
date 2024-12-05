// <copyright file="ApiAnalysisStringMatchesRegexAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisStringMatchesRegexAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisStringMatchesRegexAttribute(string pattern)
    {
        Pattern = pattern;
    }

    public string Pattern { get; }
}