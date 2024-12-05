// <copyright file="StringContains.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class StringContains
{
    public class SimpleStringClass
    {
        [ApiAnalysisStringContains("Te")]
        public string Name { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Name\":\"MrTest\"}";

        var deserialized = JsonConvert.DeserializeObject<SimpleStringClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("MrTest", deserialized.Name);
    }

    [TestMethod]
    public void DoesInclude_InMiddle_Found()
    {
        var json = "{\"Name\":\"MrTest\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DoesInclude_AtStart_Found()
    {
        var json = "{\"Name\":\"Test\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DoesInclude_AtEnd_Found()
    {
        var json = "{\"Name\":\"MrTe\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DoesNotInclude_Flagged()
    {
        var json = "{\"Name\":\"MrPest\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.ValueWasSupposedToContainMessage("MrPest", PropertyInfoHelper.Get(typeof(SimpleStringClass), nameof(SimpleStringClass.Name)), "Te"), resp.First());
    }
}