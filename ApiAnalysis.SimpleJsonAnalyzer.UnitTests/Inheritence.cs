// <copyright file="Inheritence.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class Inheritence
{
    public class BaseClass
    {
        public string Name { get; set; }

        public int Id { get; set; }
    }

    public class ChildClass : BaseClass
    {
        public string Label { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Id\":1,\"Name\":\"Fred\",\"Label\":\"Test value\"}";

        var deserialized = JsonConvert.DeserializeObject<ChildClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized.Id);
        Assert.AreEqual("Fred", deserialized.Name);
        Assert.AreEqual("Test value", deserialized.Label);
    }

    [TestMethod]
    public void InheritedProperties_HandlesOk()
    {
        var json = "{\"Id\":1,\"Name\":\"Fred\",\"Label\":\"Test value\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(ChildClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void MissingInheritedProperties_DetectedOk()
    {
        var json = "{\"Id\":1,\"Label\":\"Test value\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(ChildClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(ChildClass), nameof(ChildClass.Name))), resp.First());
    }
}