// <copyright file="StringStartsWith.cs" company="Matt Lacey">
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
    public class StringStartsWith
    {
        public class SimpleStringClass
        {
            [ApiAnalysisStringStartsWith("Te")]
            public string Name { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Name\":\"Testing\"}";

            var deserialized = JsonConvert.DeserializeObject<SimpleStringClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Testing", deserialized.Name);
        }

        [TestMethod]
        public void Does_Found()
        {
            var json = "{\"Name\":\"Testing\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void DoesNot_Flagged()
        {
            var json = "{\"Name\":\"Failing\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleStringClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.UnexpectedStartValueMessage(PropertyInfoHelper.Get(typeof(SimpleStringClass), nameof(SimpleStringClass.Name)), "Failing", "Te"), resp.First());
        }
    }
}
