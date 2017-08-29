// <copyright file="MessageBuilder.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

namespace ApiAnalysis.UnitTests
{
    /// <summary>
    /// Helper class for accessing SimpleJsonAnalyzerMessageBuilder in tests
    /// </summary>
    public static class MessageBuilder
    {
        public static SimpleJsonAnalyzerMessageBuilder Get => new SimpleJsonAnalyzerMessageBuilder();
    }
}
