// <copyright file="IgnoreTypeDifferenceFor.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests.Attributes
{
    [TestClass]
    public class IgnoreTypeDifferenceFor
    {
        public class SimpleClass
        {
            [ApiAnalysisIgnoreTypeDifferenceFor(typeof(byte))]
            public string Number { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Number\":250}";

            var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("250", deserialized.Number);
        }

        [TestMethod]
        public void Different_ButExpectedType_IsOk()
        {
            var json = "{\"Number\":250}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void Different_ButUnxpectedType_IsNotOk()
        {
            var json = "{\"Number\":25000}"; // too big for a byte so int

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleClass), nameof(SimpleClass.Number)), typeof(string), JTokenType.Integer), resp.First());
        }
    }
}
