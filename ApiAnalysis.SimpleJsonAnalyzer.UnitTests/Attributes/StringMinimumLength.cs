// <copyright file="StringMinimumLength.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class StringMinimumLength
{
    public class MinLenTestClass
    {
        [ApiAnalysisStringMinimumLength(4)]
        public string Name { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Name\":\"Testing\"}";

        var deserialized = JsonConvert.DeserializeObject<MinLenTestClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual<string>("Testing", deserialized.Name);
    }

    [TestMethod]
    public void OfDefinedLength_IsOk()
    {
        var json = "{\"Name\":\"1234\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MinLenTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void LongerThanDefinedLength_IsOk()
    {
        var json = "{\"Name\":\"12345\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MinLenTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void LessThanDefinedLength_RaisesError()
    {
        var json = "{\"Name\":\"123\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MinLenTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.InsufficientStringLengthMessage(PropertyInfoHelper.Get(typeof(MinLenTestClass), nameof(MinLenTestClass.Name)), "123", 4), resp.First());
    }

    [TestMethod]
    public void EmptyString_RaisesError()
    {
        var json = "{\"Name\":\"\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MinLenTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.InsufficientStringLengthMessage(PropertyInfoHelper.Get(typeof(MinLenTestClass), nameof(MinLenTestClass.Name)), string.Empty, 4), resp.First());
    }

    [TestMethod]
    public void Null_RaisesError()
    {
        var json = "{\"Name\":null}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MinLenTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.InsufficientStringLengthMessage(PropertyInfoHelper.Get(typeof(MinLenTestClass), nameof(MinLenTestClass.Name)), null, 4), resp.First());
    }
}