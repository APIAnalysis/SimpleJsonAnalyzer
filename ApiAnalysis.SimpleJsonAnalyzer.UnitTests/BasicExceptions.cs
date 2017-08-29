// <copyright file="BasicExceptions.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class BasicExceptions
    {
        [TestMethod]
        public void AnalyzeJsonNotPassedJson_ReportedOk()
        {
            const string json = "NOT-VALID-JSON";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(string)).Result;
            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.MissingValidJsonMessage, resp.First());
        }

        [TestMethod]
        public void AnalyzeJsonPassedNull_ReportedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(null, typeof(string)).Result;
            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.MissingValidJsonMessage, resp.First());
        }

        [TestMethod]
        public void AnalyzeJsonPassedEmptyString_ReportedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(string.Empty, typeof(string)).Result;
            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.MissingValidJsonMessage, resp.First());
        }

        public class SimpleType
        {
            public string Value { get; set; }
        }

        [TestMethod]
        public void InvalidJson_ThrowsException_ReportedOk()
        {
            var json = "{\"Value\":\"something";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleType)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.JsonAnalysisExceptionMessage(new JsonReaderException("Unterminated string. Expected delimiter: \". Path \'Value\', line 1, position 19."))));
        }
    }
}
