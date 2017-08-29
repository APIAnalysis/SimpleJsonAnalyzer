// <copyright file="ApiAnalysisStringContainsAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ApiAnalysisStringContainsAttribute : BaseApiAnalysisAttribute
    {
        public ApiAnalysisStringContainsAttribute(string expected)
        {
            this.Expected = expected;
        }

        public string Expected { get; }
    }
}
