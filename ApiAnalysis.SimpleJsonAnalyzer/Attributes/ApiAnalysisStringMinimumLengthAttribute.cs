// <copyright file="ApiAnalysisStringMinimumLengthAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace ApiAnalysis
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ApiAnalysisStringMinimumLengthAttribute : BaseApiAnalysisAttribute
    {
        public ApiAnalysisStringMinimumLengthAttribute(int minLength)
        {
            this.MinimumAcceptableLength = minLength;
        }

        public int MinimumAcceptableLength { get; }
    }
}
