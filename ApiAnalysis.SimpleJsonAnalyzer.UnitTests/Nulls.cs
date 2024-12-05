// <copyright file="Nulls.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Extras;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class Nulls
{
    public class SimpleTestClass
    {
        public string Name { get; set; }
    }

    public class TestRequiredAlwaysClass
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
    }

    public class TestAllowNullClass
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string Name { get; set; }
    }

    public class TestDisallowNullClass
    {
        [JsonProperty(Required = Required.DisallowNull)]
        public string Name { get; set; }
    }

    public class TestNullableTypeClass
    {
        public int? Value { get; set; }
    }

    public class TestNullableTypeWithListOrSingleItemConverterClass
    {
        [JsonConverter(typeof(ListOrSingleItemConverter<int?>))]
        public int? Value { get; set; }
    }

    [TestMethod]
    public void RequiredAlwaysButNull_IsDetected()
    {
        var json = "{\"Name\":null}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestRequiredAlwaysClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedNullMessage(PropertyInfoHelper.Get(typeof(TestRequiredAlwaysClass), nameof(TestRequiredAlwaysClass.Name))), resp.First());
    }

    [TestMethod]
    public void RequiredDisallowNullButNull_IsDetected()
    {
        var json = "{\"Name\":null}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestDisallowNullClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedNullMessage(PropertyInfoHelper.Get(typeof(TestDisallowNullClass), nameof(TestDisallowNullClass.Name))), resp.First());
    }

    [TestMethod]
    public void AllowNullAndMissing_IsDetected()
    {
        var json = "{}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestAllowNullClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(TestAllowNullClass), nameof(TestAllowNullClass.Name))), resp.First());
    }

    [TestMethod]
    public void NoSpecialAnnotationButNull_IsOk()
    {
        var json = "{\"Name\":null}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ServerReturningByteForNullableInt_IsOk()
    {
        var json = "{\"Value\":5}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ServerReturningIntForNullableInt_IsOk()
    {
        var json = "{\"Value\":500}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ServerReturningNullForNullableType_IsOk()
    {
        var json = "{\"Value\":null}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void NullableTypeMissing_DetectedOk()
    {
        var json = "{}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(TestNullableTypeClass), nameof(TestNullableTypeClass.Value))), resp.First());
    }

    [TestMethod]
    public void ServerReturningEmptyArrayForNullable_IsOk()
    {
        var json = "{\"Value\":[]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void ServerReturningEmptyArrayForNullString_IsOk()
    {
        var json = "{\"Name\":[]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleTestClass)).Result;

        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        Assert.AreEqual(1, resp.Count);
    }

    [TestMethod]
    public void ServerReturningEmptyObjectForNull_IsOk()
    {
        var json = "{\"Value\":{}}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TestNullableTypeClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}