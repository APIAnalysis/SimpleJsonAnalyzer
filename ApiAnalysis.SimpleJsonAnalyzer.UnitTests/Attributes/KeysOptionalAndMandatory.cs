// <copyright file="KeysOptionalAndMandatory.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApiAnalysis.UnitTests.Attributes
{
    [TestClass]
    public class KeysOptionalAndMandatory
    {
        public class OptionalClass
        {
            [ApiAnalysisOptionalKeys("aa", "bb")]
            public Dictionary<string, string> Items { get; set; }
        }

        public class MandatoryClass
        {
            [ApiAnalysisMandatoryKeys("aa", "bb")]
            public Dictionary<string, string> Items { get; set; }
        }

        public class OptionalAndMandatoryClass
        {
            [ApiAnalysisMandatoryKeys("aa", "bb")]
            [ApiAnalysisOptionalKeys("cc", "dd")]
            public Dictionary<string, string> Items { get; set; }
        }

        public class OptionalAndMandatoryWithAliasingClass
        {
            [JsonIgnore]
#pragma warning disable SA1300 // Element must begin with upper-case letter -Test needs lower-case name
#pragma warning disable IDE1006 // Naming Styles
            public Dictionary<string, string> items { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element must begin with upper-case letter

            [ApiAnalysisMandatoryKeys("aa", "bb")]
            [ApiAnalysisOptionalKeys("cc", "dd")]
            [JsonProperty("items")]
            public Dictionary<string, string> TestItems { get; set; }
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected_Optional()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var deserialized = JsonConvert.DeserializeObject<OptionalClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Items.Count);
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected_Mandatory()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var deserialized = JsonConvert.DeserializeObject<MandatoryClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Items.Count);
        }

        [TestMethod]
        public void ValidJsonDeserializesAsExpected_OptionalAndMandatory()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var deserialized = JsonConvert.DeserializeObject<OptionalAndMandatoryClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Items.Count);
        }

        [TestMethod]
        public void OptionalKeysIncluded_AcceptedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void OptionalWith_AllAndOneUnknownKey_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\", \"cc\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalClass), nameof(OptionalClass.Items)), "cc")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void OptionalWithNoDefinedKey_ReportedOk()
        {
            var json = "{\"Items\":{ \"dd\": \"good\", \"ee\": \"better\", \"ff\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalClass), nameof(OptionalClass.Items)), "dd")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalClass), nameof(OptionalClass.Items)), "ee")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalClass), nameof(OptionalClass.Items)), "ff")));
            Assert.AreEqual(3, resp.Count);
        }

        [TestMethod]
        public void OptionalKeys_OneOfTwoIncluded_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"xx\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalClass), nameof(OptionalClass.Items)), "xx")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryKeys_EmptyList_ReportedOk()
        {
            var json = "{\"Items\":{ } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(MandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "aa")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "bb")));
            Assert.AreEqual(2, resp.Count);
        }

        [TestMethod]
        public void MandatoryKeys_NoneIncludedPlusUnknowns_ReportedOk()
        {
            var json = "{\"Items\":{ \"ww\": \"good\", \"xx\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(MandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "aa")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "bb")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "ww")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "xx")));
            Assert.AreEqual(4, resp.Count);
        }

        [TestMethod]
        public void MandatoryKeys_OneOfTwoIncludedPlusUnknown_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"xx\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(MandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "bb")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "xx")));
            Assert.AreEqual(2, resp.Count);
        }

        [TestMethod]
        public void MandatoryKeys_TwoOfTwoIncluded_AcceptedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(MandatoryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryKeys_TwoOfTwoPlusUnknownIncluded_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\", \"cc\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(MandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(MandatoryClass), nameof(MandatoryClass.Items)), "cc")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_MandatoryMissingNoOptionals_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryClass), nameof(OptionalAndMandatoryClass.Items)), "bb")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_MandatoryMissingOneOptional_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"cc\": \"best\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryClass), nameof(OptionalAndMandatoryClass.Items)), "bb")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_MandatoryMissingOneOptionalPlusUnknown_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"dd\": \"best\", \"gg\": \"mystery\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.MissingMandatoryKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryClass), nameof(OptionalAndMandatoryClass.Items)), "bb")));
            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryClass), nameof(OptionalAndMandatoryClass.Items)), "gg")));
            Assert.AreEqual(2, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_AllMandatoryNoOptionals_AcceptedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_AllMandatoryOneOptional_AcceptedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\", \"dd\": \"bestest\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeys_AllMandatoryOneOptionalOneUnkown_ReportedOk()
        {
            var json = "{\"Items\":{ \"aa\": \"good\", \"bb\": \"better\", \"dd\": \"bestest\", \"tt\": \"miraculous\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryClass), nameof(OptionalAndMandatoryClass.Items)), "tt")));
            Assert.AreEqual(1, resp.Count);
        }

        [TestMethod]
        public void MandatoryAndOptionalKeysWithAliasing_AllMandatoryOneOptionalOneUnkown_ReportedOk()
        {
            var json = "{\"items\":{ \"aa\": \"good\", \"bb\": \"better\", \"dd\": \"bestest\", \"tt\": \"miraculous\" } }";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(OptionalAndMandatoryWithAliasingClass)).Result;

            Assert.IsTrue(resp.Contains(MessageBuilder.Get.UnknownKeyMessage(PropertyInfoHelper.Get(typeof(OptionalAndMandatoryWithAliasingClass), nameof(OptionalAndMandatoryWithAliasingClass.TestItems)), "tt")));
            Assert.AreEqual(1, resp.Count);
        }
    }
}
