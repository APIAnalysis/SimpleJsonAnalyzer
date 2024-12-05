// <copyright file="WorksWithAlternatePropertyNames.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class WorksWithAlternatePropertyNames
{
    public class JsonPropertyNameNamedParameterTestClass
    {
        [JsonProperty(PropertyName = "FakeName")]
        public string RealName { get; set; }
    }

    public class JsonPropertyNameDefaultConstructorTestClass
    {
        [JsonProperty("FakeName")]
        public string RealName { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class DataMemberNameTestClass
    {
        [System.Runtime.Serialization.DataMember(Name = "FakeName")]
        public string RealName { get; set; }
    }

    [TestMethod]
    public void ValidJsonPropertyDeserializesAsExpected()
    {
        var json = "{\"FakeName\":\"Joe Bloggs\"}";

        var deserialized = JsonConvert.DeserializeObject<JsonPropertyNameNamedParameterTestClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Joe Bloggs", deserialized.RealName);
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected_WithDataMemeber()
    {
        var json = "{\"FakeName\":\"Joe Bloggs\"}";

        var deserialized = JsonConvert.DeserializeObject<DataMemberNameTestClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Joe Bloggs", deserialized.RealName);
    }

    [TestMethod]
    public void JsonPropertyName_NamedParameter_HandledOk()
    {
        var json = "{\"FakeName\":\"Joe Bloggs\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(JsonPropertyNameNamedParameterTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void JsonPropertyName_DefaultConstructor_HandledOk()
    {
        var json = "{\"FakeName\":\"Joe Bloggs\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(JsonPropertyNameDefaultConstructorTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DataMemberName_HandledOk()
    {
        var json = "{\"FakeName\":\"Joe Bloggs\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(DataMemberNameTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}