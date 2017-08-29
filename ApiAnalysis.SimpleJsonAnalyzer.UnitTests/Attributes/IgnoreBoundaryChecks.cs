// <copyright file="IgnoreBoundaryChecks.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Attributes
{
    [TestClass]
    public class IgnoreBoundaryChecks
    {
        public class SimpleByteClass
        {
            [ApiAnalysisIgnoreBoundaryChecks]
            public byte Number { get; set; }
        }

        public class SimpleIntClass
        {
            [ApiAnalysisIgnoreBoundaryChecks]
            public int Number { get; set; }
        }

        public class SimpleLongClass
        {
            [ApiAnalysisIgnoreBoundaryChecks]
            public long Number { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Number\":\"92233720368547758\"}";

            var deserialized = JsonConvert.DeserializeObject<SimpleLongClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(92233720368547758, deserialized.Number);
        }

        [TestMethod]
        public void ByteBoundaryChecksCanbeIgnored()
        {
            var json = "{\"Number\":250}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleByteClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void IntegerBoundaryChecksCanbeIgnored()
        {
            var json = "{\"Number\":2147483640}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleIntClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void LongBoundaryChecksCanbeIgnored()
        {
            var json = "{\"Number\":92233720368547758}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleLongClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}
