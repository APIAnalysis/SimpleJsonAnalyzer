// <copyright file="Dates.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class Dates
{
    public class SimpleDateTimeTestClass
    {
        public DateTime Value { get; set; }
    }

    [TestMethod]
    public void DateTime_WithNoOffset_HandledOk()
    {
        var dateTimeJson = "{\"Value\":\"2016-06-24T12:34:56\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(dateTimeJson, typeof(SimpleDateTimeTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DateTime_WithZOffset_HandledOk()
    {
        var dateTimeJson = "{\"Value\":\"2016-06-24T12:34:56Z\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(dateTimeJson, typeof(SimpleDateTimeTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DateTime_WithPositiveOffset_HandledOk()
    {
        var dateTimeJson = "{\"Value\":\"2016-06-24T12:34:56+06:30\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(dateTimeJson, typeof(SimpleDateTimeTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void DateTime_WithNegativeOffset_HandledOk()
    {
        var dateTimeJson = "{\"Value\":\"2016-06-24T12:34:56-11:00\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(dateTimeJson, typeof(SimpleDateTimeTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}