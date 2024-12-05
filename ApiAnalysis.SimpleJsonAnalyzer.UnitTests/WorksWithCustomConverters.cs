// <copyright file="WorksWithCustomConverters.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ApiAnalysis.UnitTests.Extras;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class WorksWithCustomConverters
{
    public class SimpleClassUsingConverter
    {
        [JsonConverter(typeof(ListOrSingleItemConverter<string>))]
        public List<string> Items { get; set; }
    }

    public class SimpleClassUsingFailingConverter
    {
        [JsonConverter(typeof(FailingConverter))]
        public byte Items { get; set; }
    }

    private static string json = "{\"Items\":\"Item1\"}";

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var deserialized = JsonConvert.DeserializeObject<SimpleClassUsingConverter>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Items.Count);
        Assert.AreEqual("Item1", deserialized.Items.First());
    }

    [TestMethod]
    public void ArrayProperty_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassUsingConverter)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void CannotConvert_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClassUsingFailingConverter)).Result;

        Assert.AreEqual(2, resp.Count);
        Assert.IsTrue(resp.Contains(MessageBuilder.Get.JsonConverterCannotConvertMessage));
        Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleClassUsingFailingConverter), nameof(SimpleClassUsingFailingConverter.Items)), typeof(byte), JTokenType.String)));
    }
}