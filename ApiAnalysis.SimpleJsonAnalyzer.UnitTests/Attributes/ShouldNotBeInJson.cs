// <copyright file="ShouldNotBeInJson.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class ShouldNotBeInJson
{
    public class SimpleClass
    {
        [ApiAnalysisShouldNotBeInJson("reason why")]
        public string Item1 { get; set; }

        public string Item2 { get; set; }
    }

    public class SimpleClassNullReason
    {
        [ApiAnalysisShouldNotBeInJson(null)]
        public string Item1 { get; set; }

        public string Item2 { get; set; }
    }

    public class SimpleClassEmptyReason
    {
        [ApiAnalysisShouldNotBeInJson("")]
        public string Item1 { get; set; }

        public string Item2 { get; set; }
    }

    public class SimpleClassWhitespaceReason
    {
        [ApiAnalysisShouldNotBeInJson("     ")]
        public string Item1 { get; set; }

        public string Item2 { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Item2\":\"Something\"}";

        var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Something", deserialized.Item2);
    }

    [TestMethod]
    public void WhenNotIncluded_AcceptedOk()
    {
        var json = "{\"Item2\":\"Something\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void WhenIncluded_ReportedOk()
    {
        var json = "{\"Item1\":\"I'm bad\", \"Item2\":\"Something\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(MessageBuilder.Get.UnexpectedPropertyMessage(PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Item1)), "reason why"), resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void WhenIncluded_WithNullReason_ReportedOk()
    {
        var json = "{\"Item1\":\"I'm bad\", \"Item2\":\"Something\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassNullReason)).Result;

        Assert.AreEqual(MessageBuilder.Get.UnexpectedPropertyMessage(PropertyInfoHelper.Get(typeof(SimpleClassNullReason), nameof(SimpleClassNullReason.Item1)), null), resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void WhenIncluded_WithEmptyReason_ReportedOk()
    {
        var json = "{\"Item1\":\"I'm bad\", \"Item2\":\"Something\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassEmptyReason)).Result;

        Assert.AreEqual(MessageBuilder.Get.UnexpectedPropertyMessage(PropertyInfoHelper.Get(typeof(SimpleClassEmptyReason), nameof(SimpleClassEmptyReason.Item1)), string.Empty), resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void WhenIncluded_WithWhitespaceReason_ReportedOk()
    {
        var json = "{\"Item1\":\"I'm bad\", \"Item2\":\"Something\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassWhitespaceReason)).Result;

        Assert.AreEqual(MessageBuilder.Get.UnexpectedPropertyMessage(PropertyInfoHelper.Get(typeof(SimpleClassWhitespaceReason), nameof(SimpleClassWhitespaceReason.Item1)), "  "), resp.First());
        Assert.AreEqual(1, resp.Count);
    }
}