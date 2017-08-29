// <copyright file="ValidValues.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes
{
    [TestClass]
    public class ValidValues
    {
        public class SimpleClass
        {
            [ApiAnalysisValidValues("Red", "Blue", "Green")]
            public string Color { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Color\":\"Blue\"}";

            var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Blue", deserialized.Color);
        }

        [TestMethod]
        public void ExpectedValue_AcceptedOk()
        {
            var json = "{\"Color\":\"Blue\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        // should fail as change in case change from the serve may indicate a server change
        [TestMethod]
        public void ExpectedValue_ButWrongCase_DetectedAsError()
        {
            var json = "{\"Color\":\"BLUE\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.InvalidPropertyValueMessage("BLUE", PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Color))), resp.First());
        }

        [TestMethod]
        public void UnexpectedValue_DetectedOk()
        {
            var json = "{\"Color\":\"Yellow\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.InvalidPropertyValueMessage("Yellow", PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Color))), resp.First());
        }
    }
}
