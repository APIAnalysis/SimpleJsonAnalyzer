// <copyright file="ISimpleJsonAnalyzerMessageBuilder.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis
{
    public interface ISimpleJsonAnalyzerMessageBuilder
    {
        string AllGoodMessage { get; }

        string JsonConverterCannotConvertMessage { get; }

        string JsonStringIsEmptyMessage { get; }

        string MissingValidJsonMessage { get; }

        string InsufficentStringLengthMessage(PropertyInfo property, string receivedValue, int minLength);

        string UnexpectedStartValueMessage(PropertyInfo property, string receivedValue, string expectedStart);

        string UnexpectedPropertyMessage(PropertyInfo property, string reason);

        string InvalidPropertyValueMessage(string unexpectedValue, PropertyInfo property);

        string MissingPropertyValueMessage(PropertyInfo property);

        string ValueWasSupposedToContainMessage(string value, PropertyInfo property, string shouldContain);

        string ValueWasSupposedToEndWithMessage(string value, PropertyInfo property, string expectedEnding);

        string ValueWasSupposedToMatchPatternMessage(string value, PropertyInfo property, string patternToMatch);

        string ValueWasLowerThanBoundaryMessage(string value, PropertyInfo property, int boundary);

        string ValueWasHigherThanBoundaryMessage(string value, PropertyInfo property, int boundary);

        string ValueIsCloseToMaxMessage(string value, PropertyInfo property, Type type);

        string JsonIncludesUnexpectedPropertyMessage(JProperty property, Type type);

        string UnexpectedTypeMessage(PropertyInfo property, Type expectedType, JTokenType actualType);

        string ArrayOfUnexpectedTypeMessage(Type expectedType, JTokenType receivedType);

        string UnexpectedEmptyCollectionMessage(PropertyInfo property);

        string UnexpectedNullMessage(PropertyInfo property);

        string UnexpectedStringForEnumMessage(PropertyInfo property, string jsonValue);

        string UnexpectedValueForEnumMessage(PropertyInfo property, string jsonValue);

        // this can take an array of strings not PropertyInfo as they're always the names of properties of the type
        string MultipleMutuallyExclusivePropertiesMessage(Type type, params string[] properties);

        // this can take an array of strings not PropertyInfo as they're always the names of properties of the type
        string NoRequiredMutuallyExclusivePropertiesMessage(Type type, params string[] properties);

        string JsonAnalysisExceptionMessage(Exception exc);

        string UnknownKeyMessage(PropertyInfo property, string key);

        string MissingMandatoryKeyMessage(PropertyInfo property, string key);
    }
}
