// <copyright file="Bools.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class Bools
    {
        public class SimpleBoolClass
        {
            public bool Enabled { get; set; }
        }

        public class BigBoolClass
        {
            public bool LowerCaseString { get; set; }

            public bool Number { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Enabled\":true}";

            var deserialized = JsonConvert.DeserializeObject<SimpleBoolClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(true, deserialized.Enabled);
        }

        [TestMethod]
        public void AllVariationsInJsonDeserializesAsExpected()
        {
            var json = "{\"LowerCaseString\":\"true\",\"Number\":1}";

            var deserialized = JsonConvert.DeserializeObject<BigBoolClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(true, deserialized.LowerCaseString);
            Assert.AreEqual(true, deserialized.Number);
        }

        [TestMethod]
        public void True_DetectedOk()
        {
            var json = "{\"Enabled\":true}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void False_DetectedOk()
        {
            var json = "{\"Enabled\":false}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void True_AsLowercaseString_DetectedOk()
        {
            var json = "{\"Enabled\":\"true\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void False_AsLowercaseString_DetectedOk()
        {
            var json = "{\"Enabled\":\"false\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void True_AsNumber_DetectedOk()
        {
            var json = "{\"Enabled\":1}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void False_AsNumber_DetectedOk()
        {
            var json = "{\"Enabled\":0}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void String_Error_DetectedOk()
        {
            var json = "{\"Enabled\":\"NOT-A-BOOL\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleBoolClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleBoolClass), nameof(SimpleBoolClass.Enabled)), typeof(bool), JTokenType.String), resp.First());
        }
    }
}
