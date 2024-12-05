// <copyright file="ApiAnalysisValidValuesAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisValidValuesAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisValidValuesAttribute(params string[] validValues)
    {
        ValidValues = new List<string>(validValues);
    }

    public List<string> ValidValues { get; }
}