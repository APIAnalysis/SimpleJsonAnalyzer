// <copyright file="Optional.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class Optional
{
    public class OptionalTestClass
    {
        public string Name { get; set; }

        [ApiAnalysisOptional]
        public string Code { get; set; }

        [JsonIgnore]
        public string Value { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Name\":\"1a2\"}";

        var deserialized = JsonConvert.DeserializeObject<OptionalTestClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("1a2", deserialized.Name);
        Assert.IsNull(deserialized.Code);
    }

    // TODO: Need to revisit for #6
    ////[TestMethod]
    ////public void MarkedOptional_AndNotIncluded_HandledOk()
    ////{
    ////    var json = "{\"Name\":\"1a2\",\"Value\":\"1a2\"}";

    ////    var analyzer = new SimpleJsonAnalyzer();

    ////    var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalTestClass)).Result;

    ////    Assert.AreEqual(1, resp.Count);
    ////    Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    ////}

    ////[TestMethod]
    ////public void NotMarkedOptionalOrJsonIgnore_AndNotIncluded_Reported()
    ////{
    ////    var json = "{\"Value\":\"1a2\",\"Code\":\"1a2\"}";

    ////    var analyzer = new SimpleJsonAnalyzer();

    ////    var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalTestClass)).Result;

    ////    Assert.AreEqual(1, resp.Count);
    ////    Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(OptionalTestClass), nameof(OptionalTestClass.Name))), resp.First());
    ////}

    [TestMethod]
    public void MarkedJsonIgnore_AndNotIncluded_HandledOk()
    {
        var json = "{\"Name\":\"1a2\",\"Code\":\"1a2\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}