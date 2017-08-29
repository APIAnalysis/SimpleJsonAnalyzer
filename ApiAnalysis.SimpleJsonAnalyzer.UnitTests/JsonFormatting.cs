// <copyright file="JsonFormatting.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class JsonFormatting
    {
        public class SimpleClass
        {
            public string Name { get; set; }

            public int Id { get; set; }
        }

        private static string json = @"   

  {
     ""Name""  :  ""Bob"",
   ""Id"" : 1234 

       } 
      
";

        [TestMethod]
        public void ValidJsonDeserializesAsExpected()
        {
            var deserialized = JsonConvert.DeserializeObject<SimpleClass>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Bob", deserialized.Name);
            Assert.AreEqual(1234, deserialized.Id);
        }

        [TestMethod]
        public void AnyWhiteSpace_IgnoredOk()
        {
            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}
