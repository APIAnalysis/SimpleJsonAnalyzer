// <copyright file="IntegerInRange.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class IntegerInRange
{
    public class IntegerRangeCheckClass
    {
        [ApiAnalysisIntegerInRange(5, 10)]
        public int Number { get; set; }
    }

    [TestMethod]
    public void WithinRange_HandledOk()
    {
        var json = "{\"Number\":7}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(IntegerRangeCheckClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void AtLowerBoundary_HandledOk()
    {
        var json = "{\"Number\":5}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(IntegerRangeCheckClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void AtUpperBoundary_HandledOk()
    {
        var json = "{\"Number\":10}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(IntegerRangeCheckClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void BelowLowerBoundary_ReportedOk()
    {
        var json = "{\"Number\":4}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(IntegerRangeCheckClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.ValueWasLowerThanBoundaryMessage("4", PropertyInfoHelper.Get(typeof(IntegerRangeCheckClass), nameof(IntegerRangeCheckClass.Number)), 5), resp.First());
    }

    [TestMethod]
    public void AboveUpperBoundary_ReportedOk()
    {
        var json = "{\"Number\":11}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(IntegerRangeCheckClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.ValueWasHigherThanBoundaryMessage("11", PropertyInfoHelper.Get(typeof(IntegerRangeCheckClass), nameof(IntegerRangeCheckClass.Number)), 10), resp.First());
    }
}