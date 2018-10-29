// <copyright file="Tests.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiAnalysis.UnitTests.RXT
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public async Task XamFormsProfileTest()
        {
            var filePath = ".\\RXT\\Xamarin.Forms C# StackLayout.rxprofile";

            var fileContents = File.ReadAllText(filePath);

            var analyzer = new SimpleJsonAnalyzer();

            var analyzerResults = await analyzer.AnalyzeJsonAsync(fileContents, typeof(RapidXamlToolkit.Options.Profile));

            Assert.AreEqual(1, analyzerResults.Count);
            Assert.AreEqual(analyzer.MessageBuilder.AllGoodMessage, analyzerResults.First());
        }
    }
}
