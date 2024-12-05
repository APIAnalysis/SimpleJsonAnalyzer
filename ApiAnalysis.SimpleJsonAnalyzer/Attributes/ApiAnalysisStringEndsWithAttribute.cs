// <copyright file="ApiAnalysisStringEndsWithAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ApiAnalysisStringEndsWithAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisStringEndsWithAttribute(string expectedEnd)
    {
        this.ExpectedEnd = expectedEnd;
    }

    public string ExpectedEnd { get; }
}