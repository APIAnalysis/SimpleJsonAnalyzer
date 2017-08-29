// <copyright file="UnexpectedResponse.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class UnexpectedResponse
    {
        public class SimpleTestClass
        {
            public int Value { get; set; }
        }

        private const string Json = "{\"Value\":45,\"OtherValue\":99}";

        [TestMethod]
        public void JsonWithExtraProperty_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleTestClass>(Json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(45, deserialized.Value);
        }

        [TestMethod]
        public void JsonWithExtraProperty_DetectedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(Json, typeof(SimpleTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.JsonIncludesUnexpectedPropertyMessage(new JProperty("OtherValue", 99), typeof(SimpleTestClass)), resp.First());
        }
    }
}
