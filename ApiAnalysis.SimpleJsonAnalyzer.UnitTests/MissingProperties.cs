// <copyright file="MissingProperties.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class MissingProperties
{
    public class MissingPropertiesClass
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string Label { get; set; }
    }

    [TestMethod]
    public void MissingProperties_DetectedOk()
    {
        var json = "{\"Label\":\"Test value\"}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(MissingPropertiesClass)).Result;

        Assert.AreEqual(2, resp.Count);
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(MissingPropertiesClass), nameof(MissingPropertiesClass.Name))), resp.First());
        Assert.AreEqual(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(MissingPropertiesClass), nameof(MissingPropertiesClass.Id))), resp.Last());
    }
}