// <copyright file="Dictionary.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class Dictionary
    {
        public class SimpleDictionaryClass
        {
            public Dictionary<string, string> Items { get; set; }
        }

        public class ReadOnlyDictionaryClass
        {
            public IReadOnlyDictionary<string, string> Items { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Items\":{ \"one\": \"good\", \"two\": \"better\", \"three\": \"best\" } }";

            var deserialized = JsonConvert.DeserializeObject<SimpleDictionaryClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Items.Count);
        }

        [TestMethod]
        public void ValidType_HandledOk()
        {
            var json = "{\"Items\":{ \"one\": \"good\", \"two\": \"better\", \"three\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleDictionaryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void ReadOnlyType_HandledOk()
        {
            var json = "{\"Items\":{ \"one\": \"good\", \"two\": \"better\", \"three\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(ReadOnlyDictionaryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void ValidType_EmptyCollection_HandledOk()
        {
            var json = "{\"Items\":{  } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleDictionaryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void ValidType_Null_HandledOk()
        {
            var json = "{\"Items\": null }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleDictionaryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }
    }
}
