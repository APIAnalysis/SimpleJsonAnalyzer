// <copyright file="ApiAnalysisIgnoreBoundaryChecksAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis
{
    // TODO: [IDEA] add attribute for checking boundary at a different percentage level (not the default 95%)
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ApiAnalysisIgnoreBoundaryChecksAttribute : BaseApiAnalysisAttribute
    {
    }
}
