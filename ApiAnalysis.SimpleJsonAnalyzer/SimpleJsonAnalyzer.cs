// <copyright file="SimpleJsonAnalyzer.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAnalysis
{
    /// <summary>
    /// Analyzer of JSON strings and POCO Types.
    /// Doesn't have the ability to work with URIs, content links, or combine with other analyzers, like the full `JsonAnalyzer`.
    /// </summary>
    public class SimpleJsonAnalyzer
    {
        private readonly ISimpleJsonAnalyzerMessageBuilder messageBuilder;

        public SimpleJsonAnalyzer(ISimpleJsonAnalyzerMessageBuilder messageBuilder = null)
        {
            this.messageBuilder = messageBuilder ?? new SimpleJsonAnalyzerMessageBuilder();
        }

        public ISimpleJsonAnalyzerMessageBuilder MessageBuilder => this.messageBuilder;

        public async Task<List<string>> AnalyzeJsonAsync(string jsonToAnalyze, Type typeToCompareWith)
        {
            if (string.IsNullOrEmpty(jsonToAnalyze))
            {
                return new List<string> { this.messageBuilder.MissingValidJsonMessage };
            }

            return await this.AnalyzeJsonInternalAsync(jsonToAnalyze, typeToCompareWith);
        }

        private void PerformChecksRelativeToJsonPropertyRequired(PropertyInfo pocoProperty, JProperty jsonProperty, List<string> result)
        {
            var jsonPropertyAttribute = pocoProperty.GetCustomAttribute(typeof(JsonPropertyAttribute));

            if (jsonPropertyAttribute != null)
            {
                var required = ((JsonPropertyAttribute)jsonPropertyAttribute).Required;

                if (jsonProperty.Value.Type == JTokenType.Null)
                {
                    if (required == Required.Always || required == Required.DisallowNull)
                    {
                        result.Add(this.messageBuilder.UnexpectedNullMessage(pocoProperty));
                    }
                }
            }
        }

        private string GetAlternatePocoType(Attribute ignoreType, bool knowJsonType, PropertyInfo pocoProperty, string pocoPropertyType)
        {
            var result = "{undefined}";

            if (ignoreType != null)
            {
                result = (ignoreType as ApiAnalysisIgnoreTypeDifferenceForAttribute)?.JsonType.Name;
            }

            if (knowJsonType)
            {
                if (pocoProperty.PropertyType.IsGenericType)
                {
                    var toAppendToPoco = "<" + pocoProperty.PropertyType.GenericTypeArguments[0].Name + ">";

                    result = pocoPropertyType + toAppendToPoco;
                }
                else if (pocoProperty.PropertyType.IsArray)
                {
                    result = "Array<" + pocoProperty.PropertyType.Name.Replace("[]", string.Empty) + ">";
                }
            }
            else if (pocoProperty.PropertyType.Name == "Nullable`1")
            {
                result = pocoProperty.PropertyType.GenericTypeArguments[0].Name;
            }

            return result;
        }

        private void CheckForConstraintOnMatchingRegex(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var matchesRegex = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisStringMatchesRegexAttribute));

            if (matchesRegex is ApiAnalysisStringMatchesRegexAttribute stringMatchesRegexAttribute)
            {
                var patternToMatch = stringMatchesRegexAttribute.Pattern;

                if (!Regex.IsMatch(jsonPropertyValue.ToString(), patternToMatch))
                {
                    result.Add(this.messageBuilder.ValueWasSupposedToMatchPatternMessage(jsonPropertyValue.ToString(), pocoProperty, patternToMatch));
                }
            }
        }

        private void CheckForConstraintOnStringMinimumLength(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var minLength = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisStringMinimumLengthAttribute));

            var minAcceptableLength = (minLength as ApiAnalysisStringMinimumLengthAttribute)?.MinimumAcceptableLength;

            if (jsonPropertyValue.ToString().Trim().Length < minAcceptableLength)
            {
                result.Add(this.messageBuilder.InsufficentStringLengthMessage(pocoProperty, jsonPropertyValue.ToString(), minAcceptableLength.Value));
            }
        }

        private void CheckForConstraintOnStringStart(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var startsWith = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisStringStartsWithAttribute));

            var expectedPrefix = (startsWith as ApiAnalysisStringStartsWithAttribute)?.ExpectedStart;

            if (expectedPrefix != null && !jsonPropertyValue.ToString().StartsWith(expectedPrefix, StringComparison.Ordinal))
            {
                result.Add(this.messageBuilder.UnexpectedStartValueMessage(pocoProperty, jsonPropertyValue.ToString(), expectedPrefix));
            }
        }

        private void CheckForConstraintOnStingEnd(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var endsWith = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisStringEndsWithAttribute));

            if (endsWith is ApiAnalysisStringEndsWithAttribute apiAnalysisStringEndsWithAttribute)
            {
                var expectedSuffix = apiAnalysisStringEndsWithAttribute.ExpectedEnd;

                if (!jsonPropertyValue.ToString().EndsWith(expectedSuffix, StringComparison.Ordinal))
                {
                    result.Add(this.messageBuilder.ValueWasSupposedToEndWithMessage(jsonPropertyValue.ToString(), pocoProperty, expectedSuffix));
                }
            }
        }

        private void CheckForConstraintOnStringContains(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var contains = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisStringContainsAttribute));

            if (contains is ApiAnalysisStringContainsAttribute apiAnalysisStringContainsAttribute)
            {
                var shouldContain = apiAnalysisStringContainsAttribute.Expected;

                if (!jsonPropertyValue.ToString().Contains(shouldContain))
                {
                    result.Add(this.messageBuilder.ValueWasSupposedToContainMessage(jsonPropertyValue.ToString(), pocoProperty, shouldContain));
                }
            }
        }

        private void CheckForByteCloseToBoundary(string pocoPropertyType, PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            if ((pocoPropertyType == typeof(byte).Name)
                && !pocoProperty.HasCustomAttribute(typeof(ApiAnalysisIgnoreBoundaryChecksAttribute))
                && ((byte)jsonPropertyValue > (byte.MaxValue * .95)))
            {
                result.Add(this.messageBuilder.ValueIsCloseToMaxMessage(jsonPropertyValue.ToString(), pocoProperty, typeof(byte)));
            }
        }

        private void CheckForIntegerCloseToBoundary(string pocoPropertyType, PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            if ((pocoPropertyType == typeof(int).Name)
                && !pocoProperty.HasCustomAttribute(typeof(ApiAnalysisIgnoreBoundaryChecksAttribute))
                && ((int)jsonPropertyValue > (int.MaxValue * .95)))
            {
                result.Add(this.messageBuilder.ValueIsCloseToMaxMessage(jsonPropertyValue.ToString(), pocoProperty, typeof(int)));
            }
        }

        private void CheckForLongCloseToBoundary(string pocoPropertyType, PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            if ((pocoPropertyType == typeof(long).Name)
                && !pocoProperty.HasCustomAttribute(typeof(ApiAnalysisIgnoreBoundaryChecksAttribute))
                && ((long)jsonPropertyValue > (long.MaxValue * .95)))
            {
                result.Add(this.messageBuilder.ValueIsCloseToMaxMessage(jsonPropertyValue.ToString(), pocoProperty, typeof(long)));
            }
        }

        private void CheckForValidListOfValues(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var validValues = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisValidValuesAttribute));

            var apiAnalysisValidValuesAttribute = validValues as ApiAnalysisValidValuesAttribute;

            var accepted = apiAnalysisValidValuesAttribute?.ValidValues;

            if (!accepted?.Contains(jsonPropertyValue.ToString()) == true)
            {
                result.Add(this.messageBuilder.InvalidPropertyValueMessage(jsonPropertyValue.ToString(), pocoProperty));
            }
        }

        private void CheckForIntegersInDefinedRange(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
            var rangeCheck = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisIntegerInRangeAttribute));

            if (rangeCheck is ApiAnalysisIntegerInRangeAttribute apiAnalysisIntegerInRangeAttribute)
            {
                var lowestAllowed = apiAnalysisIntegerInRangeAttribute.Lowest;

                if ((int)jsonPropertyValue < lowestAllowed)
                {
                    result.Add(this.messageBuilder.ValueWasLowerThanBoundaryMessage(jsonPropertyValue.ToString(), pocoProperty, lowestAllowed));
                }
                else
                {
                    var highestAllowed = apiAnalysisIntegerInRangeAttribute.Highest;

                    if ((int)jsonPropertyValue > highestAllowed)
                    {
                        result.Add(this.messageBuilder.ValueWasHigherThanBoundaryMessage(jsonPropertyValue.ToString(), pocoProperty, highestAllowed));
                    }
                }
            }
        }

        private Type GenerateSimpleTypeWithJsonConverter(PropertyInfo pocoProperty, JsonConverterAttribute jsonConverter)
        {
            var assemblyName = new AssemblyName { Name = "tmpAssembly" };
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assemblyBuilder.DefineDynamicModule("tmpModule");

            var typeBuilder = module.DefineType("TempConverterType", TypeAttributes.Public | TypeAttributes.Class);

            var propertyName = pocoProperty.Name;
            var propertyType = pocoProperty.PropertyType;

            var privateBackingField = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var publicProperty = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, new Type[] { });

            var ci = typeof(JsonConverterAttribute).GetConstructor(new[] { typeof(Type) });

            if (ci != null)
            {
                var cab = new CustomAttributeBuilder(ci, new object[] { jsonConverter.ConverterType });

                publicProperty.SetCustomAttribute(cab);
            }

            var getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;

            var getterMethod = typeBuilder.DefineMethod("get_value", getterAndSetterAttributes, propertyType, Type.EmptyTypes);

            var getterMethodIl = getterMethod.GetILGenerator();
            getterMethodIl.Emit(OpCodes.Ldarg_0);
            getterMethodIl.Emit(OpCodes.Ldfld, privateBackingField);
            getterMethodIl.Emit(OpCodes.Ret);

            var setterMethod = typeBuilder.DefineMethod("set_value", getterAndSetterAttributes, null, new[] { propertyType });

            var setterMethodIl = setterMethod.GetILGenerator();
            setterMethodIl.Emit(OpCodes.Ldarg_0);
            setterMethodIl.Emit(OpCodes.Ldarg_1);
            setterMethodIl.Emit(OpCodes.Stfld, privateBackingField);
            setterMethodIl.Emit(OpCodes.Ret);

            publicProperty.SetGetMethod(getterMethod);
            publicProperty.SetSetMethod(setterMethod);

            var genType = typeBuilder.CreateType();
            return genType;
        }

        // Change POCO types to their JSON equivalent where they differ
        private string StandardizeTypeName(string typeName)
        {
            return typeName.Replace("Integer", "Int32")
                .Replace("IList`", "List`")
                .Replace("IReadOnlyList`1", "Array")
                .Replace("List`1", "Array")
                .Replace("IEnumerable`1", "Array")
                .Replace("IReadOnlyDictionary`2", "Dictionary");
        }

        private async Task<List<string>> AnalyzeJsonInternalAsync(string jsonToAnalyze, Type typeToCompareWith, bool isRecursive = false, PropertyInfo recursiveProperty = null)
        {
            var result = new List<string>();

            try
            {
                // handle new lines, etc. at start of json
                jsonToAnalyze = jsonToAnalyze.Trim();

                if (string.IsNullOrWhiteSpace(jsonToAnalyze))
                {
                    // TODO: this should also be better handled after the HTTP request and with a more appropriate message (e.g. server returned no content for {uri})
                    result.Add(this.messageBuilder.JsonStringIsEmptyMessage);
                }
                else if (jsonToAnalyze.StartsWith("[", StringComparison.InvariantCultureIgnoreCase))
                {
                    var arrayGeneratedFromJson = JArray.Parse(jsonToAnalyze);

                    foreach (var jobject in arrayGeneratedFromJson)
                    {
                        Type comparisonType;

                        if (typeToCompareWith.IsArray)
                        {
                            comparisonType = typeToCompareWith.GetElementType();
                        }
                        else if (typeToCompareWith.IsGenericType)
                        {
                            // Note this only handles generics of a single type (e.g. not a Tuple)
                            comparisonType = typeToCompareWith.GetGenericArguments().Single();
                        }
                        else
                        {
                            comparisonType = typeToCompareWith;
                            Debug.WriteLine("Not sure how it's possible to get here. If you do, please provide a sample of the JSON and POCO Type so we can add test coverage for this.");
                        }

                        if (jobject is JObject jobj)
                        {
                            result.AddRange(await this.AnalyzeJObjectAsync(jobj, comparisonType, isRecursive: true));
                        }
                        else
                        {
                            if (jobject.Type.ToString() != comparisonType.Name)
                            {
                                var msg = this.messageBuilder.ArrayOfUnexpectedTypeMessage(comparisonType, jobject.Type);

                                if (!result.Contains(msg))
                                {
                                    result.Add(msg);
                                }
                            }
                        }
                    }
                }
                else if (jsonToAnalyze.StartsWith("{", StringComparison.InvariantCultureIgnoreCase))
                {
                    var objectGeneratedFromJson = JObject.Parse(jsonToAnalyze);
                    result.AddRange(await this.AnalyzeJObjectAsync(objectGeneratedFromJson, typeToCompareWith, isRecursive, recursiveProperty));
                }
                else
                {
                    result.Add(this.messageBuilder.MissingValidJsonMessage);
                }
            }
            catch (Exception ex)
            {
                result.Add(this.messageBuilder.JsonAnalysisExceptionMessage(ex));
            }

            if (result.Count == 0 && !isRecursive)
            {
                result.Add(this.messageBuilder.AllGoodMessage);
            }

            return result;
        }

        private async Task<List<string>> AnalyzeJObjectAsync(JObject objectToAnalyze, Type typeToCompareWith, bool isRecursive = false, PropertyInfo recursiveProperty = null)
        {
            var result = new List<string>();

            try
            {
                var foundProperties = new List<string>();
                var foundMutuallyExclusiveProperty = string.Empty;

                foreach (var jsonProperty in objectToAnalyze.Properties())
                {
                    // CHECK: property in JSON exists in POCO
                    var pocoProperty = this.GetProperty(typeToCompareWith, jsonProperty.Name);

                    if (pocoProperty != null)
                    {
                        foundProperties.Add(jsonProperty.Name.ToLowerInvariant());

                        if (pocoProperty.GetCustomAttribute(typeof(ApiAnalysisRequiredMutuallyExclusiveAttribute)) != null)
                        {
                            if (string.IsNullOrEmpty(foundMutuallyExclusiveProperty))
                            {
                                foundMutuallyExclusiveProperty = pocoProperty.Name;
                            }
                            else
                            {
                                result.Add(this.messageBuilder.MultipleMutuallyExclusivePropertiesMessage(typeToCompareWith, foundMutuallyExclusiveProperty, pocoProperty.Name));
                            }
                        }

                        var shouldNotBeInJson = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisShouldNotBeInJsonAttribute));

                        if (shouldNotBeInJson is ApiAnalysisShouldNotBeInJsonAttribute snbij)
                        {
                            result.Add(this.messageBuilder.UnexpectedPropertyMessage(pocoProperty, snbij.Reason));
                        }

                        var pocoPropertyType = pocoProperty.PropertyType.Name;

                        var jsonPropertyType = jsonProperty.Value.Type.ToString();

                        if (jsonPropertyType == typeof(object).Name)
                        {
                            if (new[] { "Dictionary`2", "IReadOnlyDictionary`2" }.Contains(pocoPropertyType))
                            {
                                // NOTE. only report unknown keys if there are mandatory or optional keys defined
                                var mandatoryKeys = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisMandatoryKeysAttribute)) as ApiAnalysisMandatoryKeysAttribute;

                                var optionalKeys = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisOptionalKeysAttribute)) as ApiAnalysisOptionalKeysAttribute;

                                if (jsonProperty.Value.HasValues)
                                {
                                    Debug.WriteLine("handle dictionary items");

                                    if (mandatoryKeys != null || optionalKeys != null)
                                    {
                                        var allKeys = new List<string>();

                                        if (optionalKeys != null)
                                        {
                                            allKeys.AddRange(optionalKeys.OptionalKeys);
                                        }

                                        if (mandatoryKeys != null)
                                        {
                                            allKeys.AddRange(mandatoryKeys.MandatoryKeys);
                                        }

                                        // Check for any Unknown keys
                                        foreach (var jsonEntry in jsonProperty.Value.Children())
                                        {
                                            var key = ((JProperty)jsonEntry).Name;

                                            if (!allKeys.Contains(key))
                                            {
                                                result.Add(this.messageBuilder.UnknownKeyMessage(pocoProperty, key));
                                            }
                                        }

                                        if (mandatoryKeys != null)
                                        {
                                            // Check all mandatory keys included
                                            foreach (var key in mandatoryKeys.MandatoryKeys)
                                            {
                                                if (jsonProperty.Value.Children().All(c => ((JProperty)c).Name != key))
                                                {
                                                    result.Add(this.messageBuilder.MissingMandatoryKeyMessage(pocoProperty, key));
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // JSON is an empty object
                                    if (mandatoryKeys != null && mandatoryKeys.MandatoryKeys.Any())
                                    {
                                        result.AddRange(
                                            mandatoryKeys.MandatoryKeys.Select(key => this.messageBuilder
                                                .MissingMandatoryKeyMessage(pocoProperty, key)));
                                    }
                                }
                            }
                            else
                            {
                                result.AddRange(await this.AnalyzeJsonInternalAsync(
                                    jsonProperty.Value.ToString(),
                                    pocoProperty.PropertyType,
                                    isRecursive: true,
                                    recursiveProperty: pocoProperty));
                            }

                            continue;
                        }

                        if (jsonPropertyType == JTokenType.Integer.ToString())
                        {
                            // Be more specific about type for a numeric value
                            var jsonValue = long.Parse(jsonProperty.Value.ToString());

                            if (jsonValue <= byte.MaxValue)
                            {
                                jsonPropertyType = typeof(byte).Name;
                            }
                            else
                            if (jsonValue <= int.MaxValue)
                            {
                                jsonPropertyType = typeof(int).Name;
                            }
                            else
                            {
                                jsonPropertyType = typeof(long).Name;
                            }
                        }

                        if (jsonPropertyType == JTokenType.Date.ToString())
                        {
                            jsonPropertyType = ((JValue)jsonProperty.Value).Value.GetType().Name;
                        }

                        bool knowJsonType = false;

                        if (jsonPropertyType == JTokenType.Array.ToString())
                        {
                            if (jsonProperty.Value.Any())
                            {
                                var nestedTypeResult = new List<string>();

                                Type pocoType;

                                if (pocoProperty.PropertyType.IsArray)
                                {
                                    pocoType = pocoProperty.PropertyType.GetElementType();
                                }
                                else if (pocoProperty.PropertyType.IsGenericType)
                                {
                                    // Note this only handles generic lists of a single type (e.g. not a Tuple)
                                    pocoType = pocoProperty.PropertyType.GetGenericArguments().Single();
                                }
                                else
                                {
                                    pocoType = pocoProperty.PropertyType;
                                }

                                var pocoTypeProps = pocoType.GetProperties();

                                if (pocoTypeProps.Any() && pocoType != typeof(string))
                                {
                                    foreach (var nestedJsonObject in jsonProperty.Value)
                                    {
                                        nestedTypeResult.AddRange(await this.AnalyzeJsonInternalAsync(nestedJsonObject.ToString(), pocoType, isRecursive: true, recursiveProperty: pocoProperty));
                                    }
                                }
                                else
                                {
                                    foreach (var nestedJsonObject in jsonProperty.Value)
                                    {
                                        if (!this.TypesAreTheSameOrEquivalent(nestedJsonObject.Type.ToString(), pocoType.Name))
                                        {
                                            var msg = this.messageBuilder.ArrayOfUnexpectedTypeMessage(pocoType, nestedJsonObject.Type);

                                            if (!result.Contains(msg))
                                            {
                                                result.Add(msg);
                                            }
                                        }
                                    }
                                }

                                knowJsonType = true;

                                if (nestedTypeResult.Any())
                                {
                                    result.AddRange(nestedTypeResult);
                                }

                                if (pocoProperty.PropertyType.IsArray)
                                {
                                    jsonPropertyType += "<" + pocoProperty.PropertyType.GetElementType().Name + ">";
                                }
                                else if (pocoProperty.PropertyType.IsGenericType)
                                {
                                    jsonPropertyType += "<" + pocoProperty.PropertyType.GetGenericArguments().Single().Name + ">";
                                }
                                else
                                {
                                    jsonPropertyType = pocoProperty.PropertyType.Name + "[]";
                                }
                            }
                            else
                            {
                                var noEmptyCollectionsAttribute = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisCollectionMayNotBeEmptyAttribute));

                                if (noEmptyCollectionsAttribute != null)
                                {
                                    result.Add(this.messageBuilder.UnexpectedEmptyCollectionMessage(pocoProperty));
                                }
                            }
                        }

                        this.PerformChecksRelativeToJsonPropertyRequired(pocoProperty, jsonProperty, result);

                        var ignoreType = pocoProperty.GetCustomAttribute(typeof(ApiAnalysisIgnoreTypeDifferenceForAttribute));

                        var alternatePocoType = this.GetAlternatePocoType(ignoreType, knowJsonType, pocoProperty, pocoPropertyType);

                        // CHECK: types of matching property names are the same
                        if (!this.TypesAreTheSameOrEquivalent(jsonPropertyType, pocoPropertyType)
                            && !this.TypesAreTheSameOrEquivalent(jsonPropertyType, alternatePocoType)
                            && !this.DataCanAutoConvertToType(jsonProperty, pocoPropertyType, pocoProperty)
                            && !this.DataCanAutoConvertToType(jsonProperty, alternatePocoType)
                            && !this.CanProcessWithConverter(jsonProperty, pocoProperty, result))
                        {
                            result.Add(this.messageBuilder.UnexpectedTypeMessage(pocoProperty, pocoProperty.PropertyType, jsonProperty.Value.Type));
                        }
                        else
                        {
                            var jsonPropertyValue = jsonProperty.Value;

                            // Avoid checks dependent on attributes if none is present
                            if (pocoProperty.HasCustomAttribute(typeof(BaseApiAnalysisAttribute)))
                            {
                                this.CheckForConstraintOnStringMinimumLength(pocoProperty, jsonPropertyValue, result);
                                this.CheckForConstraintOnStringStart(pocoProperty, jsonPropertyValue, result);
                                this.CheckForConstraintOnStingEnd(pocoProperty, jsonPropertyValue, result);
                                this.CheckForConstraintOnStringContains(pocoProperty, jsonPropertyValue, result);
                                this.CheckForConstraintOnMatchingRegex(pocoProperty, jsonPropertyValue, result);
                                this.CheckForIntegersInDefinedRange(pocoProperty, jsonPropertyValue, result);
                                this.CheckForValidListOfValues(pocoProperty, jsonPropertyValue, result);
                                await this.CheckForValidContentOfTypesRetrievedFromUriAsync(pocoProperty, jsonPropertyValue, result);
                                await this.CheckForValidContentOfTypesRetrievedFromUriConditionallyAsync(pocoProperty, jsonPropertyValue, objectToAnalyze, result);
                            }

                            // These checks are based on having AND not having an attribute so must be outside the above test
                            this.CheckForByteCloseToBoundary(pocoPropertyType, pocoProperty, jsonPropertyValue, result);
                            this.CheckForIntegerCloseToBoundary(pocoPropertyType, pocoProperty, jsonPropertyValue, result);
                            this.CheckForLongCloseToBoundary(pocoPropertyType, pocoProperty, jsonPropertyValue, result);
                        }
                    }
                    else
                    {
                        result.Add(this.messageBuilder.JsonIncludesUnexpectedPropertyMessage(jsonProperty, typeToCompareWith));
                    }
                }

                var exclusiveProperties = typeToCompareWith.GetProperties().Where(p => p.GetCustomAttribute(typeof(ApiAnalysisRequiredMutuallyExclusiveAttribute)) != null).ToArray();

                if (string.IsNullOrEmpty(foundMutuallyExclusiveProperty))
                {
                    if (exclusiveProperties.Any())
                    {
                        result.Add(this.messageBuilder.NoRequiredMutuallyExclusivePropertiesMessage(typeToCompareWith, exclusiveProperties.Select(p => p.Name).ToArray()));
                    }
                }

                // CHECK: No properties expected in Type are missing from JSON
                foreach (var propertyInfo in typeToCompareWith.GetProperties())
                {
                    // CHECK: Can handle properties that are optional in POCO
                    if (propertyInfo.HasCustomAttribute(typeof(ApiAnalysisOptionalAttribute))
                        || propertyInfo.HasCustomAttribute(typeof(ApiAnalysisShouldNotBeInJsonAttribute))
                        || propertyInfo.HasCustomAttribute(typeof(JsonIgnoreAttribute)))
                    {
                        continue;
                    }

                    // If not found this but have found its mutually exclusive counterpart that is fine
                    if (propertyInfo.GetCustomAttribute(typeof(ApiAnalysisRequiredMutuallyExclusiveAttribute)) != null
                        && !string.IsNullOrEmpty(foundMutuallyExclusiveProperty))
                    {
                        continue;
                    }

                    // CHECK: Property has a setter - if it doesn't then we don't expect a value for it in the JSON
                    if (propertyInfo.SetMethod == null)
                    {
                        continue;
                    }

                    // CHECK: Missing property is part of a declared subset
                    string propertySubsetName = null;
                    var propertySubset = propertyInfo.GetCustomAttribute(typeof(ApiAnalysisPartOfSubsetAttribute));

                    if (propertySubset != null)
                    {
                        propertySubsetName = ((ApiAnalysisPartOfSubsetAttribute)propertySubset).SubsetName;
                    }

                    string typeSubsetName = null;
                    var typeSubset = recursiveProperty?.GetCustomAttribute(typeof(ApiAnalysisSubsetAttribute));

                    if (typeSubset != null)
                    {
                        typeSubsetName = ((ApiAnalysisSubsetAttribute)typeSubset).SubsetName;
                    }

                    if (typeSubsetName != null && typeSubsetName != propertySubsetName)
                    {
                        continue;
                    }

                    // CHECK: Not already found it & not already found another MutEx prop if this is MutEx too
                    // The JSON.Net default is to map on matching case and then case insensitively if a match is not found
                    //  this is just quicker. (Ignoring the case where may have properties with the same name but different cases)
                    if (!foundProperties.Contains(propertyInfo.Name.ToLowerInvariant())
                        && !foundProperties.Contains(propertyInfo.JsonPropertyName()?.ToLowerInvariant() ?? "DoesNotExist")
                        && !foundProperties.Contains(propertyInfo.DataMemberName()?.ToLowerInvariant() ?? "DoesNotExist")
                        && exclusiveProperties.All(p => !string.Equals(p.Name, propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        result.Add(this.messageBuilder.MissingPropertyValueMessage(propertyInfo));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Add(this.messageBuilder.JsonAnalysisExceptionMessage(ex));
            }

            // Don't include the all good message for nested types
            if (result.Count == 0 && !isRecursive)
            {
                result.Add(this.messageBuilder.AllGoodMessage);
            }

            return result;
        }

#pragma warning disable SA1202 // Elements must be ordered by access

        // This is not supported in SimpleJsonAnalyzer
        protected virtual async Task CheckForValidContentOfTypesRetrievedFromUriConditionallyAsync(PropertyInfo pocoProperty, JToken jsonPropertyValue, JObject objectToAnalyze, List<string> result)
        {
#if NET45
            // This does nothing but returns a completed task and can be awaited to keep the compiler happy and consistent across framework versions
            await Task.WhenAll(new List<Task>());
#else
            await Task.CompletedTask;
#endif
        }

        // This is not supported in SimpleJsonAnalyzer
        protected virtual async Task CheckForValidContentOfTypesRetrievedFromUriAsync(PropertyInfo pocoProperty, JToken jsonPropertyValue, List<string> result)
        {
#if NET45
            // This does nothing but returns a completed task and can be awaited to keep the compiler happy and consistent across framework versions
            await Task.WhenAll(new List<Task>());
#else
            await Task.CompletedTask;
#endif
        }
#pragma warning restore SA1202 // Elements must be ordered by access

        private bool CanProcessWithConverter(JProperty jsonProperty, PropertyInfo pocoProperty, List<string> result)
        {
            var jsonConverter = pocoProperty.GetCustomAttribute(typeof(JsonConverterAttribute));

            if (jsonConverter != null)
            {
                var converter = (JsonConverter)Activator.CreateInstance(((JsonConverterAttribute)jsonConverter).ConverterType);

                if (!converter.CanConvert(pocoProperty.PropertyType))
                {
                    result.Add(this.messageBuilder.JsonConverterCannotConvertMessage);
                }
                else
                {
                    try
                    {
                        var stringToConvert = "{" + jsonProperty + "}";

                        var genType = this.GenerateSimpleTypeWithJsonConverter(pocoProperty, (JsonConverterAttribute)jsonConverter);

                        // Deserialize to check can do this
                        var ignored = JsonConvert.DeserializeObject(stringToConvert, genType);

                        Debug.WriteLine(ignored);
                        return true; // We successfully processed with the converter
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        result.Add(this.messageBuilder.UnexpectedValueForEnumMessage(pocoProperty, jsonProperty.Value.ToString()));
                        return true; // Returning true as have dealt with conversion (even though it errored)
                    }
                }
            }
            else
            {
                // Allow for automatic enum conversion
                if (pocoProperty.PropertyType.IsEnum)
                {
                    if (jsonProperty.Value.Type == JTokenType.String)
                    {
                        foreach (var name in Enum.GetNames(pocoProperty.PropertyType))
                        {
                            if (string.Equals(name, jsonProperty.Value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                return true;
                            }
                        }

                        result.Add(this.messageBuilder.UnexpectedStringForEnumMessage(pocoProperty, jsonProperty.Value.ToString()));
                        return true; // Returning true as have dealt with conversion
                    }

                    if (jsonProperty.Value.Type == JTokenType.Integer)
                    {
                        foreach (var value in Enum.GetValues(pocoProperty.PropertyType))
                        {
                            if (Convert.ChangeType(value, Enum.GetUnderlyingType(pocoProperty.PropertyType)).ToString() == jsonProperty.Value.ToString())
                            {
                                return true;
                            }
                        }

                        result.Add(this.messageBuilder.UnexpectedValueForEnumMessage(pocoProperty, jsonProperty.Value.ToString()));
                        return true; // Returning true as have dealt with conversion
                    }
                }
            }

            return false;
        }

        private bool DataCanAutoConvertToType(JProperty jsonProperty, string pocoPropertyName, PropertyInfo propertyInfo = null)
        {
            if (propertyInfo != null && jsonProperty.Value.Type == JTokenType.Null)
            {
                return !propertyInfo.PropertyType.IsValueType || (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null);
            }

            switch (pocoPropertyName)
            {
                case "Boolean":
                    if ((jsonProperty.Value.Type == JTokenType.Integer)
                        && (jsonProperty.Value.ToString() == "0" || jsonProperty.Value.ToString() == "1"))
                    {
                        return true;
                    }

                    if (jsonProperty.Value.Type == JTokenType.String
                        && (jsonProperty.Value.ToString() == "false" || jsonProperty.Value.ToString() == "true"))
                    {
                        return true;
                    }

                    break;

                case "Uri":
                    return Uri.TryCreate(jsonProperty.Value.ToString(), UriKind.Absolute, out Uri ignored);
            }

            // Allow an empty array to be treated as null - because some APIs work that way!
            if (jsonProperty.Value.ToString() == "[]")
            {
                if (propertyInfo?.PropertyType.IsClass == true
                    || pocoPropertyName == "Nullable`1")
                {
                    return true;
                }
            }

            return false;
        }

        private bool TypesAreTheSameOrEquivalent(string type1, string type2)
        {
            if (type1 == type2)
            {
                return true;
            }

            // empty array (of unknown type) is comparable with an array of a known type
            if (type1 == JTokenType.Array.ToString()
                && type2.EndsWith("[]", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (type2 == typeof(byte).Name)
            {
                return type1 == JTokenType.Integer.ToString();
            }

            // it is ok if we have interpreted a JSON Integer value
            // as something that fits in a larger POCO type
            if (type2 == typeof(int).Name)
            {
                return type1 == typeof(byte).Name;
            }

            if (type2 == typeof(long).Name)
            {
                return type1 == typeof(int).Name
                       || type1 == typeof(byte).Name;
            }

            if (type2 == typeof(decimal).Name)
            {
                return type1 == JTokenType.Float.ToString()
                       || type1 == typeof(float).Name;
            }

            if (type2 == typeof(double).Name)
            {
                return type1 == JTokenType.Float.ToString()
                       || type1 == typeof(float).Name;
            }

            if (type2 == typeof(float).Name)
            {
                return type1 == typeof(decimal).Name
                       || type1 == typeof(double).Name
                       || type1 == JTokenType.Float.ToString();
            }

            return this.StandardizeTypeName(type1) == this.StandardizeTypeName(type2);
        }

        private PropertyInfo GetProperty(Type type, string propertyName)
        {
            PropertyInfo result = null;

            foreach (var propertyInfo in type.GetProperties())
            {
                // We match case insensitively, like default behaviour in JSON.Net
                if (string.Equals(propertyInfo.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = propertyInfo;
                    break;
                }

                // CHECK: can handle property names that are mapped in the POCO Type
                // check for JsonProperty(PropertyName=XXX)
                if (propertyInfo.HasCustomAttributeAndProperty(nameof(JsonPropertyAttribute), nameof(JsonPropertyAttribute.PropertyName), propertyName))
                {
                    result = propertyInfo;
                    break;
                }

                // check for DataMember(Name=XXX)
                if (propertyInfo.HasCustomAttributeAndProperty(nameof(DataMemberAttribute), nameof(DataMemberAttribute.Name), propertyName))
                {
                    result = propertyInfo;
                    break;
                }
            }

            return result;
        }
    }
}
