// <copyright file="ApiAnalysisMandatoryKeysAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisMandatoryKeysAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisMandatoryKeysAttribute(params string[] mandatoryKeys)
    {
        this.MandatoryKeys = new List<string>(mandatoryKeys);
    }

    public List<string> MandatoryKeys { get; }
}