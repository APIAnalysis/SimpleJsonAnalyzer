// <copyright file="WorksWithSumTypes.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

// SumTypes like a Discriminated Union
[TestClass]
public class WorksWithSumTypes
{
    public class TestErrorClass
    {
        public int Code { get; set; }

        public string Details { get; set; }
    }

    public class TestDetailsClass
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class SimpleProductClass
    {
        [ApiAnalysisRequiredMutuallyExclusive]
        public TestDetailsClass Details { get; set; }

        [ApiAnalysisRequiredMutuallyExclusive]
        public TestErrorClass Error { get; set; }
    }

    public class SingleExclusivePropertyClass
    {
        [ApiAnalysisRequiredMutuallyExclusive]
        public TestDetailsClass Details { get; set; }
    }

    public class TripleMutExPropertyClass
    {
        [ApiAnalysisRequiredMutuallyExclusive]
        public TestDetailsClass One { get; set; }

        [ApiAnalysisRequiredMutuallyExclusive]
        public TestDetailsClass Two { get; set; }

        [ApiAnalysisRequiredMutuallyExclusive]
        public TestDetailsClass Three { get; set; }
    }

    private static string detailsJson = "{\"Details\":{\"Id\":987,\"Name\":\"TestItem\"}}";
    private static string errorJson = "{\"Error\":{\"Code\":689,\"Details\":\"Internal Server Config Error\"}}";
    private static string detailsAndErrorJson = "{\"Details\":{\"Id\":987,\"Name\":\"TestItem\"},\"Error\":{\"Code\":689,\"Details\":\"Internal Server Config Error\"}}";

    [TestMethod]
    public void ValidJsonDeserializes_Details_AsExpected()
    {
        var deserialized = JsonConvert.DeserializeObject<SimpleProductClass>(detailsJson);

        Assert.IsNotNull(deserialized);
        Assert.IsNotNull(deserialized.Details);
        Assert.IsNull(deserialized.Error);
        Assert.AreEqual(987, deserialized.Details.Id);
        Assert.AreEqual("TestItem", deserialized.Details.Name);
    }

    [TestMethod]
    public void ValidJsonDeserializes_Error_AsExpected()
    {
        var deserialized = JsonConvert.DeserializeObject<SimpleProductClass>(errorJson);

        Assert.IsNotNull(deserialized);
        Assert.IsNotNull(deserialized.Error);
        Assert.IsNull(deserialized.Details);
        Assert.AreEqual(689, deserialized.Error.Code);
        Assert.AreEqual("Internal Server Config Error", deserialized.Error.Details);
    }

    [TestMethod]
    public void JustDetails_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(detailsJson, typeof(SimpleProductClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void JustErrors_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(detailsJson, typeof(SimpleProductClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void BothDetailsAndError_ReportedOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(detailsAndErrorJson, typeof(SimpleProductClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MultipleMutuallyExclusivePropertiesMessage(typeof(SimpleProductClass), nameof(SimpleProductClass.Details), nameof(SimpleProductClass.Error)), resp.First());
    }

    [TestMethod]
    public void NeitherDetailsOrError_ReportedOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{}", typeof(SimpleProductClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.NoRequiredMutuallyExclusivePropertiesMessage(typeof(SimpleProductClass), nameof(SimpleProductClass.Details), nameof(SimpleProductClass.Error)), resp.First());
    }

    [TestMethod]
    public void SingleExclusiveProperty_Returned_HandledOk()
    {
        // Shouldn't ever have the mutuallyexclusive attribute on a single property - just want to test it doesn't break anything if you do
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(detailsJson, typeof(SingleExclusivePropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void SingleExclusiveProperty_Missing_ReportedOk()
    {
        // Shouldn't ever have the mutuallyexclusive attribute on a single property - just want to test it doesn't break anything if you do
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{}", typeof(SingleExclusivePropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.NoRequiredMutuallyExclusivePropertiesMessage(typeof(SingleExclusivePropertyClass), nameof(SingleExclusivePropertyClass.Details)), resp.First());
    }

    // If have Mutex attributes on more than two properties it should require only one is specified
    [TestMethod]
    public void TripleMutexProperty_Missing_ReportedOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{}", typeof(TripleMutExPropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.NoRequiredMutuallyExclusivePropertiesMessage(typeof(TripleMutExPropertyClass), nameof(TripleMutExPropertyClass.One), nameof(TripleMutExPropertyClass.Three), nameof(TripleMutExPropertyClass.Two)), resp.First());
    }

    [TestMethod]
    public void TripleMutexProperty_OneSet_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{\"One\":{\"Id\":987,\"Name\":\"TestItem\"}}", typeof(TripleMutExPropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void TripleMutexProperty_TwoSet_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{\"Two\":{\"Id\":987,\"Name\":\"TestItem\"}}", typeof(TripleMutExPropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void TripleMutexProperty_ThreeSet_HandledOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync("{\"Three\":{\"Id\":987,\"Name\":\"TestItem\"}}", typeof(TripleMutExPropertyClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void TripleMutexProperty_AllThreeSet_ReportedOk()
    {
        var analyzer = new SimpleJsonAnalyzer();

        var json = "{\"One\":{\"Id\":987,\"Name\":\"TestItem\"},\"Two\":{\"Id\":987,\"Name\":\"TestItem\"},\"Three\":{\"Id\":987,\"Name\":\"TestItem\"}}";

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(TripleMutExPropertyClass)).Result;

        Assert.AreEqual(2, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MultipleMutuallyExclusivePropertiesMessage(typeof(TripleMutExPropertyClass), nameof(TripleMutExPropertyClass.One), nameof(TripleMutExPropertyClass.Two)), resp.First());
        Assert.AreEqual(MessageBuilder.Get.MultipleMutuallyExclusivePropertiesMessage(typeof(TripleMutExPropertyClass), nameof(TripleMutExPropertyClass.One), nameof(TripleMutExPropertyClass.Three)), resp.Last());
    }
}