// <copyright file="BasicTypes.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class BasicTypes
    {
        public class SimpleByteTestClass
        {
            public byte Value { get; set; }
        }

        private const string ByteJson = "{\"Value\":45}";

        [TestMethod]
        public void ClassWith_Byte_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleByteTestClass>(ByteJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(45, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Byte_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(ByteJson, typeof(SimpleByteTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleIntTestClass
        {
            public int Value { get; set; }
        }

        private const string IntJson = "{\"Value\":4500}";

        [TestMethod]
        public void ClassWith_Int_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleIntTestClass>(IntJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(4500, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Int_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(IntJson, typeof(SimpleIntTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleLongTestClass
        {
            public long Value { get; set; }
        }

        private const string LongJson = "{\"Value\":87622034350120366}";

        [TestMethod]
        public void ClassWith_Long_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleLongTestClass>(LongJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(87622034350120366, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Long_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(LongJson, typeof(SimpleLongTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleBoolTestClass
        {
            public bool Value { get; set; }
        }

        private const string BoolJson = "{\"Value\":true}";

        [TestMethod]
        public void ClassWith_Bool_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleBoolTestClass>(BoolJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(true, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Bool_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(BoolJson, typeof(SimpleBoolTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleStringTestClass
        {
            public string Value { get; set; }
        }

        private const string StringJson = "{\"Value\":\"abcde\"}";

        [TestMethod]
        public void ClassWith_String_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleStringTestClass>(StringJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("abcde", deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_String_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(StringJson, typeof(SimpleStringTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleArrayTestClass
        {
            public byte[] Value { get; set; }
        }

        private const string ArrayJson = "{\"Value\":[4,5,6]}";

        [TestMethod]
        public void ClassWith_Array_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleArrayTestClass>(ArrayJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Value.Length);
        }

        [TestMethod]
        public void ClassWith_Array_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(ArrayJson, typeof(SimpleArrayTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleListTestClass
        {
            public List<byte> Value { get; set; }
        }

        private const string ListJson = "{\"Value\":[4,5,6]}";

        [TestMethod]
        public void ClassWith_List_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleListTestClass>(ListJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Value.Count);
        }

        [TestMethod]
        public void ClassWith_List_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(ListJson, typeof(SimpleListTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleUriTestClass
        {
            public Uri Value { get; set; }
        }

        private const string UriJson = "{\"Value\":\"http://mrlacey.com/\"}";

        [TestMethod]
        public void ClassWith_Uri_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleUriTestClass>(UriJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(new Uri("http://mrlacey.com/").AbsoluteUri, deserialized.Value.AbsoluteUri);
        }

        [TestMethod]
        public void ClassWith_Uri_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(UriJson, typeof(SimpleUriTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        [TestMethod]
        public void ClassWith_InvalidUri_DetectedCorrectly()
        {
            string invalidUriJson = "{\"Value\":\"not-a-valid-uri\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(invalidUriJson, typeof(SimpleUriTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.UnexpectedTypeMessage(PropertyInfoHelper.Get(typeof(SimpleUriTestClass), nameof(SimpleUriTestClass.Value)), typeof(Uri), JTokenType.String), resp.First());
        }

        public class SimpleFloatTestClass
        {
            public float Value { get; set; }
        }

        private const string FloatJson = "{\"Value\":1.23}";

        [TestMethod]
        public void ClassWith_Float_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleFloatTestClass>(FloatJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(1.23, deserialized.Value, 0.000001);
        }

        [TestMethod]
        public void ClassWith_Float_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(FloatJson, typeof(SimpleFloatTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleDoubleTestClass
        {
            public double Value { get; set; }
        }

        private const string DoubleJson = "{\"Value\":2.34}";

        [TestMethod]
        public void ClassWith_Double_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleDoubleTestClass>(DoubleJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2.34, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Double_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(DoubleJson, typeof(SimpleDoubleTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleDecimalTestClass
        {
            public decimal Value { get; set; }
        }

        private const string DecimalJson = "{\"Value\":3.45}";

        [TestMethod]
        public void ClassWith_Decimal_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleDecimalTestClass>(DecimalJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3.45M, deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_Decimal_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(DecimalJson, typeof(SimpleDecimalTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }

        public class SimpleDateTimeTestClass
        {
            public DateTime Value { get; set; }
        }

        private const string DateTimeJson = "{\"Value\":\"2016-06-24T12:34:56Z\"}";

        [TestMethod]
        public void ClassWith_DateTime_DeserializesOk()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleDateTimeTestClass>(DateTimeJson);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(new DateTime(2016, 6, 24, 12, 34, 56), deserialized.Value);
        }

        [TestMethod]
        public void ClassWith_DateTime_PropertyCheckedOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(DateTimeJson, typeof(SimpleDateTimeTestClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}
