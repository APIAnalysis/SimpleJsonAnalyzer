// <copyright file="TypeDifferences.cs" company="Matt Lacey">
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
    public class TypeDifferences
    {
        public class TypeDifferenceTestClass
        {
            public string Value { get; set; }
        }

        public class TypeDifferenceWithAttributeTestClass
        {
            [ApiAnalysisIgnoreTypeDifferenceFor(typeof(int))]
            public string Value { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Value\":123}";

            var deserialized = JsonConvert.DeserializeObject<TypeDifferenceWithAttributeTestClass>(json);

            // JSON.net will convert to a string
            Assert.IsNotNull(deserialized);
            Assert.AreEqual("123", deserialized.Value);
        }

        [TestMethod]
        public void AttributedTypeDifferences_AreAccepted()
        {
            var json = "{\"Value\":123}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(TypeDifferenceWithAttributeTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void UnexpectedTypeDifferences_AreDetected()
        {
            var json = "{\"Value\":12345}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(TypeDifferenceTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(TypeDifferenceTestClass), nameof(TypeDifferenceTestClass.Value)), typeof(string), JTokenType.Integer), resp.First());
        }
    }
}
