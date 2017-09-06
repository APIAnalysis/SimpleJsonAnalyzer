// <copyright file="MessageBuilderTests.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis.UnitTests
{
    [TestClass]
    public class MessageBuilderTests
    {
        [TestMethod]
        public void DefaultMessageBuilder_UsedByDefault()
        {
            var analyzer = new SimpleJsonAnalyzer();

            Assert.AreEqual(analyzer.MessageBuilder.GetType(), typeof(SimpleJsonAnalyzerMessageBuilder));
        }

#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar - Suppression is fine as don't require implementation in tests.
        public class TestCustomMessageBuilder : ISimpleJsonAnalyzerMessageBuilder
        {
            public string AllGoodMessage => "I exist only for testing";

            public string JsonConverterCannotConvertMessage { get; }

            public string JsonStringIsEmptyMessage { get; }

            public string MissingValidJsonMessage { get; }

            public string UnexpectedStartValueMessage(PropertyInfo property, string receivedValue, string expectedStart)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedPropertyMessage(PropertyInfo property, string reason)
            {
                throw new NotImplementedException();
            }

            public string InvalidPropertyValueMessage(string unexpectedValue, PropertyInfo property)
            {
                throw new NotImplementedException();
            }

            public string MissingPropertyValueMessage(PropertyInfo property)
            {
                throw new NotImplementedException();
            }

            public string ValueWasSupposedToContainMessage(string value, PropertyInfo property, string shouldContain)
            {
                throw new NotImplementedException();
            }

            public string ValueWasSupposedToEndWithMessage(string value, PropertyInfo property, string expectedEnding)
            {
                throw new NotImplementedException();
            }

            public string ValueWasSupposedToMatchPatternMessage(string value, PropertyInfo property, string patternToMatch)
            {
                throw new NotImplementedException();
            }

            public string ValueWasLowerThanBoundaryMessage(string value, PropertyInfo property, int boundary)
            {
                throw new NotImplementedException();
            }

            public string ValueWasHigherThanBoundaryMessage(string value, PropertyInfo property, int boundary)
            {
                throw new NotImplementedException();
            }

            public string ValueIsCloseToMaxMessage(string value, PropertyInfo property, Type type)
            {
                throw new NotImplementedException();
            }

            public string JsonIncludesUnexpectedPropertyMessage(JProperty property, Type type)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedTypeMessage(PropertyInfo property, Type expectedType, JTokenType actualType)
            {
                throw new NotImplementedException();
            }

            public string ArrayOfUnexpectedTypeMessage(Type expectedType, JTokenType receivedType)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedEmptyCollectionMessage(PropertyInfo property)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedNullMessage(PropertyInfo property)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedStringForEnumMessage(PropertyInfo property, string jsonValue)
            {
                throw new NotImplementedException();
            }

            public string UnexpectedValueForEnumMessage(PropertyInfo property, string jsonValue)
            {
                throw new NotImplementedException();
            }

            public string MultipleMutuallyExclusivePropertiesMessage(Type type, params string[] properties)
            {
                throw new NotImplementedException();
            }

            public string NoRequiredMutuallyExclusivePropertiesMessage(Type type, params string[] properties)
            {
                throw new NotImplementedException();
            }

            public string JsonAnalysisExceptionMessage(Exception exc)
            {
                throw new NotImplementedException();
            }

            public string UnknownKeyMessage(PropertyInfo property, string key)
            {
                throw new NotImplementedException();
            }

            public string MissingMandatoryKeyMessage(PropertyInfo property, string key)
            {
                throw new NotImplementedException();
            }
        }
#pragma warning restore RECS0083 // Shows NotImplementedException throws in the quick task bar

        [TestMethod]
        public void CustomMessageBuilder_ExposedIfSpecified()
        {
            var testMessageBuilder = new TestCustomMessageBuilder();

            var analyzer = new SimpleJsonAnalyzer(testMessageBuilder);

            Assert.AreEqual(analyzer.MessageBuilder.GetType(), typeof(TestCustomMessageBuilder));
            Assert.AreEqual(analyzer.MessageBuilder.AllGoodMessage, testMessageBuilder.AllGoodMessage);
        }
    }
}
