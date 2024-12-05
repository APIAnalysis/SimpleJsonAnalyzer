// <copyright file="Lists.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class Lists
{
    public class SimpleListStringPropertyClass
    {
        public string Items { get; set; }
    }

    public class SimpleListTestClass
    {
        public List<string> Items { get; set; }
    }

    public class SimpleListTestWithNestedClass
    {
        public List<Child> Kids { get; set; }
    }

    public class Child
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Items\":[\"one\", \"two\"]}";

        var deserialized = JsonConvert.DeserializeObject<SimpleListTestClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Items.Count);
        Assert.AreEqual("one", deserialized.Items.First());
        Assert.AreEqual("two", deserialized.Items.Last());
    }

    [TestMethod]
    public void OfSimpleTypes_HandledOk()
    {
        var json = "[\"one\", \"two\"]";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(List<string>)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void OfWrongSimpleTypes_HandledOk()
    {
        var json = "[3, 4, 5]";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(List<string>)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.ArrayOfUnexpectedTypeMessage(typeof(string), JTokenType.Integer), resp.First());
    }

    [TestMethod]
    public void OfComplexTypes_HandledOk()
    {
        var json = "[{\"Name\":\"Jonny\",\"Age\":7}, {\"Name\":\"Sally\",\"Age\":13}]";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(List<Child>)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void EmptyValue_HandledOk()
    {
        var json = "[]";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(List<string>)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void EmptyValueAsProperty_HandledOk()
    {
        var json = "{\"Items\":[]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void AsProperty_HandledOk()
    {
        var json = "{\"Items\":[\"one\", \"two\"]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void OfDifferentTypes_ReportedOk()
    {
        var json = "{\"Items\":[1, 2]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.ArrayOfUnexpectedTypeMessage(typeof(string), JTokenType.Integer), resp.First());
    }

    [TestMethod]
    public void ExpectedButNotReturned_ReportedOk()
    {
        var json = "{}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(SimpleListTestClass), nameof(SimpleListTestClass.Items))), resp.First());
    }

    [TestMethod]
    public void ReturnedButNotExpected_ReportedOk()
    {
        var json = "{\"Items\":[\"one\", \"two\"]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListStringPropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleListStringPropertyClass), nameof(SimpleListStringPropertyClass.Items)), typeof(string), JTokenType.Array), resp.First());
    }
}