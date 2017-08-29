// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicValidJsonParsing.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class BasicValidJsonParsing
    {
        public class SimpleClass
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public bool Enabled { get; set; }

            public float Score { get; set; }

            public Uri Website { get; set; }
        }

        [TestMethod]
        public void SimpleClassLooksOk()
        {
            var json = "{\"Id\":1,\"Name\":\"Fred\",\"Enabled\":true,\"Score\":99.999,\"Website\":\"http://www.example.com\"}";

            var analyzer = new SimpleJsonAnalyzer();

            var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleClass)).Result;

            Assert.AreEqual(1, resp.Count);
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}
