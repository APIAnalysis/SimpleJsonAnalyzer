// <copyright file="SimpleJsonAnalyzerMessageBuilder.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis;

public class SimpleJsonAnalyzerMessageBuilder : ISimpleJsonAnalyzerMessageBuilder
{
    public string AllGoodMessage => "All looks good.";

    public string MissingValidJsonMessage => "Invalid JSON could not be analyzed.";

    public string JsonConverterCannotConvertMessage => "The JsonConverter was not able to convert the returned value.";

    public string JsonStringIsEmptyMessage => "JSON string contains no content.";

    public string JsonAnalysisExceptionMessage(Exception exc)
    {
        return $"Exception during JSON analysis: {exc.Message}";
    }

    public string ArrayOfUnexpectedTypeMessage(Type expectedType, JTokenType receivedType)
    {
        return $"Expected an array of '{expectedType.Name}' but JSON contained '{receivedType}'.";
    }

    public string InvalidPropertyValueMessage(string unexpectedValue, PropertyInfo property)
    {
        return ExceptionalValueMessage(unexpectedValue, property, "was not a valid value.");
    }

    public string JsonIncludesUnexpectedPropertyMessage(JProperty property, Type type)
    {
        return $"The JSON included the property '{property.Name}' (with value '{property.Value}') but it does not exist in class '{type.Name}'.";
    }

    public string MissingMandatoryKeyMessage(PropertyInfo property, string key)
    {
        return $"Expected '{property.DeclaringType?.Name}.{property.Name}' to include the key '{key}'.";
    }

    public string MissingPropertyValueMessage(PropertyInfo property)
    {
        return $"Expected '{property.DeclaringType?.Name}' to have a property named '{property.Name}' but it did not exist in JSON.";
    }

    public string MultipleMutuallyExclusivePropertiesMessage(Type type, params string[] properties)
    {
        var result = $"JSON for '{type.Name}' included mutually exclusive properties: ";

        return properties.Aggregate(result, (current, property) => current + property + ", ").TrimEnd(',', ' ');
    }

    public string NoRequiredMutuallyExclusivePropertiesMessage(Type type, params string[] properties)
    {
        var result = $"JSON for '{type.Name}' did not include any of the required mutually exclusive properties: ";

        return properties.OrderBy(p => p).Aggregate(result, (current, property) => current + property + ", ");
    }

    public string UnexpectedTypeMessage(PropertyInfo property, Type expectedType, JTokenType actualType)
    {
        return $"Expected '{property.Name}' to be of type '{expectedType.Name}' but server returned a '{actualType}'.";
    }

    public string UnexpectedEmptyCollectionMessage(PropertyInfo property)
    {
        return $"The JSON contained an unexpected empty collection for '{property.Name}'.";
    }

    public string UnexpectedNullMessage(PropertyInfo property)
    {
        return $"The JSON contained an unexpected Null value for '{property.Name}'.";
    }

    public string UnexpectedPropertyMessage(PropertyInfo property, string reason)
    {
        return string.IsNullOrWhiteSpace(reason)
            ? $"The JSON contained the property '{property.Name}' but it should not."
            : $"The JSON contained the property '{property.Name}' but it should not because '{reason}'.";
    }

    public string InsufficientStringLengthMessage(PropertyInfo property, string receivedValue, int minLength)
    {
        return $"The value '{receivedValue}' (of property '{property.Name}') was supposed to be at least {minLength} characters long.";
    }

    public string UnexpectedStartValueMessage(PropertyInfo property, string receivedValue, string expectedStart)
    {
        return ExceptionalValueMessage(receivedValue, property, "was supposed to start with", expectedStart);
    }

    public string UnexpectedStringForEnumMessage(PropertyInfo property, string jsonValue)
    {
        return $"The property '{property.Name}' is an '{property.PropertyType.Name}' but the JSON contained unknown value '{jsonValue}'.";
    }

    public string UnexpectedValueForEnumMessage(PropertyInfo property, string jsonValue)
    {
        return $"The property '{property.Name}' is of type '{property.PropertyType.Name}' but the JSON contained the value '{jsonValue}' which could not be converted.";
    }

    public string UnknownKeyMessage(PropertyInfo property, string key)
    {
        return $"'{property.DeclaringType?.Name}.{property.Name}' included the unexpected key '{key}'.";
    }

    public string ValueWasHigherThanBoundaryMessage(string value, PropertyInfo property, int boundary)
    {
        return ExceptionalValueMessage(value, property, "should not be higher than", boundary.ToString());
    }

    public string ValueIsCloseToMaxMessage(string value, PropertyInfo property, Type type)
    {
        return ExceptionalValueMessage(value, property, "is getting close to the max value for a", type.Name);
    }

    public string ValueWasLowerThanBoundaryMessage(string value, PropertyInfo property, int boundary)
    {
        return ExceptionalValueMessage(value, property, "should not be lower than", boundary.ToString());
    }

    public string ValueWasSupposedToContainMessage(string value, PropertyInfo property, string shouldContain)
    {
        return ExceptionalValueMessage(value, property, "was supposed to contain", shouldContain);
    }

    public string ValueWasSupposedToEndWithMessage(string value, PropertyInfo property, string expectedEnding)
    {
        return ExceptionalValueMessage(value, property, "was supposed to end with", expectedEnding);
    }

    public string ValueWasSupposedToMatchPatternMessage(string value, PropertyInfo property, string patternToMatch)
    {
        return ExceptionalValueMessage(value, property, "was supposed to match with", patternToMatch);
    }

    private string ExceptionalValueMessage(string value, PropertyInfo property, string expectedCondition, string suffix = null)
    {
        var result = $"The value '{value}' (of property '{property.Name}') {expectedCondition}";

        if (suffix != null)
        {
            result = $"{result} '{suffix}'.";
        }

        return result;
    }
}