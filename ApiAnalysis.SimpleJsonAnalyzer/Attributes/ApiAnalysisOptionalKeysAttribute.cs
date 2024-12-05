// <copyright file="ApiAnalysisOptionalKeysAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace ApiAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiAnalysisOptionalKeysAttribute : BaseApiAnalysisAttribute
{
    public ApiAnalysisOptionalKeysAttribute(params string[] optionalKeys)
    {
        OptionalKeys = new List<string>(optionalKeys);
    }

    public List<string> OptionalKeys { get; }
}