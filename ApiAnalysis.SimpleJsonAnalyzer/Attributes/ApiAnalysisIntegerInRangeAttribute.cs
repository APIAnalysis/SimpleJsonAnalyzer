// <copyright file="ApiAnalysisIntegerInRangeAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisIntegerInRangeAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisIntegerInRangeAttribute(int lowest, int highest)
    {
        this.Lowest = lowest;
        this.Highest = highest;
    }

    public int Lowest { get; }

    public int Highest { get; }
}