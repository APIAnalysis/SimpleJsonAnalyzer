// <copyright file="ReadOnlyProperties.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class ReadOnlyProperties
{
    public class SimpleReadOnlyTestClass
    {
        public int Value { get; set; }

        public string FormattedValue => $"--{this.Value}--";
    }

    private string json = "{\"Value\":1}";

    [TestMethod]
    public void ValidJsonDeserializes_AsExpected()
    {
        var deserialized = JsonConvert.DeserializeObject<SimpleReadOnlyTestClass>(this.json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Value);
        Assert.AreEqual("--1--", deserialized.FormattedValue);
    }

    [TestMethod]
    public void ReadOnlyValueNotInJson_AcceptedOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(this.json, typeof(SimpleReadOnlyTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}