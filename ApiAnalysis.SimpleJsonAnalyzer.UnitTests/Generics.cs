// <copyright file="Generics.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class Generics
    {
        public class SimpleKvpClass
        {
            public KeyValuePair<int, string> Info { get; set; }
        }

        [TestMethod]
        public void KeyValuePair_HandledOk()
        {
            var json = "{\"Info\":{\"Key\":1,\"Value\":\"two\"}}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleKvpClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleTupleClass
        {
            public Tuple<int, string> Info { get; set; }
        }

        [TestMethod]
        public void Tuple_DeserializesOk()
        {
            var json = "{\"Info\":{\"Item1\":1,\"Item2\":\"two\"}}";

            var deserialized = JsonConvert.DeserializeObject<SimpleTupleClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.Info);
            Assert.AreEqual(1, deserialized.Info.Item1);
            Assert.AreEqual("two", deserialized.Info.Item2);
        }

        [TestMethod]
        public void Tuple_HandledOk()
        {
            var json = "{\"Info\":{\"Item1\":1,\"Item2\":\"two\"}}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleTupleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}
