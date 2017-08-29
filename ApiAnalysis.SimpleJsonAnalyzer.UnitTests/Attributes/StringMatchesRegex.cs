// <copyright file="StringMatchesRegex.cs" company="Matt Lacey">
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
    public class StringMatchesRegex
    {
        public class SimpleStringClass
        {
            [ApiAnalysisStringMatchesRegex(@"^\d{1}[a-z]{1}\d{1}$")]
            public string Code { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Code\":\"1a2\"}";

            var deserialized = JsonConvert.DeserializeObject<SimpleStringClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("1a2", deserialized.Code);
        }

        [TestMethod]
        public void Does_Found()
        {
            var json = "{\"Code\":\"1a2\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void DoesNot_Flagged()
        {
            var json = "{\"Code\":\"a1a\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.ValueWasSupposedToMatchPatternMessage("a1a", PropertyInfoHelper.Get(typeof(SimpleStringClass), nameof(SimpleStringClass.Code)), @"^\d{1}[a-z]{1}\d{1}$"), resp.First());
        }
    }
}
