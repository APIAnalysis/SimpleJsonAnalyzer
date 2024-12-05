// <copyright file="CollectionMayNotBeEmpty.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests.Attributes;

[TestClass]
public class CollectionMayNotBeEmpty
{
    public class SimpleArrayTestClass
    {
        [ApiAnalysisCollectionMayNotBeEmpty]
        public string[] Items { get; set; }
    }

    public class SimpleListTestClass
    {
        [ApiAnalysisCollectionMayNotBeEmpty]
        public List<string> Items { get; set; }
    }

    [TestMethod]
    public void EmptyArray_DetectedOk()
    {
        var json = "{\"Items\":[]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleArrayTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedEmptyCollectionMessage(PropertyInfoHelper.Get(typeof(SimpleArrayTestClass), nameof(SimpleArrayTestClass.Items))), resp.First());
    }

    [TestMethod]
    public void EmptyList_DetectedOk()
    {
        var json = "{\"Items\":[]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.UnexpectedEmptyCollectionMessage(PropertyInfoHelper.Get(typeof(SimpleListTestClass), nameof(SimpleListTestClass.Items))), resp.First());
    }

    [TestMethod]
    public void ArrayThatCannotBeEmptyAndPopulated_HandledOk()
    {
        var json = "{\"Items\":[\"something\"]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleArrayTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }

    [TestMethod]
    public void ListThatCannotBeEmptyAndPopulated_HandledOk()
    {
        var json = "{\"Items\":[\"something\"]}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleListTestClass)).Result;

        Assert.AreEqual(1, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
    }
}