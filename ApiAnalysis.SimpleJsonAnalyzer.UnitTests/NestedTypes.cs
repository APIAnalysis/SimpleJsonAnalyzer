// <copyright file="NestedTypes.cs" company="Matt Lacey">
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
    public class NestedTypes
    {
        public class SimpleOuterType
        {
            public string Value { get; set; }

            public InnerType Inner { get; set; }
        }

        public class OuterTypeWithArray
        {
            public string Value { get; set; }

            public InnerType[] Inner { get; set; }
        }

        public class InnerType
        {
            public string Name { get; set; }

            public int Id { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var json = "{\"Value\":\"something\",\"Inner\":{\"Name\":\"Bob\",\"Id\":1234}}";

            var deserialized = JsonConvert.DeserializeObject<SimpleOuterType>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("something", deserialized.Value);
            Assert.IsNotNull(deserialized.Inner);
            Assert.AreEqual("Bob", deserialized.Inner.Name);
            Assert.AreEqual(1234, deserialized.Inner.Id);
        }

        [TestMethod]
        public void ValidJson_WithArray_DeserializesAsExpected()
        {
            var json = "{\"Value\":\"something\",\"Inner\":[{\"Name\":\"Bob\",\"Id\":1234},{\"Name\":\"Sue\",\"Id\":9876}]}";

            var deserialized = JsonConvert.DeserializeObject<OuterTypeWithArray>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("something", deserialized.Value);
            Assert.IsNotNull(deserialized.Inner);
            Assert.AreEqual(2, deserialized.Inner.Length);
        }

        [TestMethod]
        public void ArrayProperty_HandledOk()
        {
            var json = "{\"Value\":\"something\",\"Inner\":[{\"Name\":\"Bob\",\"Id\":1234},{\"Name\":\"Sue\",\"Id\":9876}]}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OuterTypeWithArray)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void EmptySubType_ReportedOk()
        {
            var json = "{\"Value\":\"something\",\"Inner\":{}}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleOuterType)).Result;

            Assert.AreEqual(2, resp.Count);
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(InnerType), nameof(InnerType.Name)))));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingPropertyValueMessage(PropertyInfoHelper.Get(typeof(InnerType), nameof(InnerType.Id)))));
        }

        [TestMethod]
        public void SubTypeAsEmptyString_ReportedOk()
        {
            var json = "{\"Value\":\"something\",\"Inner\":\"\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleOuterType)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleOuterType), nameof(SimpleOuterType.Inner)), typeof(InnerType), JTokenType.String)));
        }

        [TestMethod]
        public void SubTypeHasError_ReportedOk()
        {
            var json = "{\"Value\":\"something\",\"Inner\":{\"Name\":\"Bob\",\"Id\":\"not-an-int\"}}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleOuterType)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(InnerType), nameof(InnerType.Id)), typeof(int), JTokenType.String)));
        }

        [TestMethod]
        public void SubTypeInArrayHasError_ReportedOk()
        {
            var json = "{\"Value\":\"something\",\"Inner\":[{\"Name\":\"Bob\",\"Id\":\"not-an-int\"},{\"Name\":\"Sue\",\"Id\":9876}]}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OuterTypeWithArray)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(InnerType), nameof(InnerType.Id)), typeof(int), JTokenType.String)));
        }
    }
}
