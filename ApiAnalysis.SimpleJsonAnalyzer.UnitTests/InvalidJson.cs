// <copyright file="InvalidJson.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class InvalidJson
{
    public class SimpleClass
    {
        public string Name { get; set; }

        public int Id { get; set; }
    }

    [TestMethod]
    public void NoClosingBrace_IsReported()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var json = @"{""Name"" : ""Bob"", ""Id"" : 1234";

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual("Exception during JSON analysis: Unexpected end of content while loading JObject. Path 'Id', line 1, position 28.", resp.First());
    }

    [TestMethod]
    public void InvalidJson_IsReported()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var json = @"{""Name"" = ""Bob"", ""Id"" = 1234}";

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual("Exception during JSON analysis: Invalid character after parsing property name. Expected ':' but got: =. Path '', line 1, position 8.", resp.First());
    }
}