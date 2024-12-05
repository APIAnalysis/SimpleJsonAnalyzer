// <copyright file="Enums.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class Enums
{
    public enum ValidColors
    {
        Red,
        Green,
        Blue,
    }

    public class SimpleClass
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ValidColors Color { get; set; }
    }

    public class SimpleClassNoAttribute
    {
        public ValidColors Color { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializes_String_AsExpected()
    {
        var json = "{\"Color\":\"Blue\"}";

        var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(ValidColors.Blue, deserialized.Color);
    }

    [TestMethod]
    public void ValidJsonDeserializes_Int_AsExpected()
    {
        var json = "{\"Color\":2}";

        var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(ValidColors.Blue, deserialized.Color);
    }

    [TestMethod]
    public void ValidJsonDeserializes_StringWithoutAttribute_AsExpected()
    {
        var json = "{\"Color\":\"Blue\"}";

        var deserialized = JsonConvert.DeserializeObject<SimpleClassNoAttribute>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(ValidColors.Blue, deserialized.Color);
    }

    [TestMethod]
    public void ValidJsonDeserializes_IntWithoutAttribute_AsExpected()
    {
        var json = "{\"Color\":2}";

        var deserialized = JsonConvert.DeserializeObject<SimpleClassNoAttribute>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(ValidColors.Blue, deserialized.Color);
    }

    [TestMethod]
    public void ExpectedValueWithAttribute_AcceptedOk()
    {
        var json = "{\"Color\":\"Blue\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ExpectedValueWithoutAttribue_AcceptedOk()
    {
        var json = "{\"Color\":\"Blue\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassNoAttribute)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ValidInt_WithAttribute_AcceptedOk()
    {
        var json = "{\"Color\":2}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ValidInt_WithoutAttribue_AcceptedOk()
    {
        var json = "{\"Color\":2}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassNoAttribute)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void UnexpectedStringValue_DetectedOk()
    {
        var json = "{\"Color\":\"Yellow\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedValueForEnumMessage(PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Color)), "Yellow"), resp.First());
    }

    [TestMethod]
    public void UnexpectedStringValueWithoutAttribute_DetectedOk()
    {
        var json = "{\"Color\":\"Yellow\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassNoAttribute)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedStringForEnumMessage(PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Color)), "Yellow"), resp.First());
    }

    [TestMethod]
    public void UnexpectedIntValue_DetectedOk()
    {
        var json = "{\"Color\":3}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());

        // Note: StringEnumConverter will allow values of the underlying type that do not map to a specified/named value (autoconverting doesn't)
        // Technically this test is valid but may expect the following response. If that's the case could remove use of StringEnumConverter
        // Assert.AreEqual(MessageBuilder.UnexpectedValueForEnum(nameof(SimpleClass.Color), nameof(ValidColors), "3"), resp.First());
    }

    [TestMethod]
    public void UnexpectedIntValueWithoutAttribute_DetectedOk()
    {
        var json = "{\"Color\":3}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassNoAttribute)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedValueForEnumMessage(PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Color)), "3"), resp.First());
    }
}