// <copyright file="ListOrSingleItemConverter.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Extras
{
    // This converter will treat single values like a list of that item with a single value
#pragma warning disable SA1649 // File name must match first type name
    public class ListOrSingleItemConverter<T> : JsonConverter
#pragma warning restore SA1649 // File name must match first type name
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar
            throw new NotImplementedException(
                "Unnecessary as CanWrite is false. This converter does not handle writing and so this converter will be skipped when writing.");
#pragma warning restore RECS0083 // Shows NotImplementedException throws in the quick task bar
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.StartArray
                ? serializer.Deserialize<List<T>>(reader)
                : new List<T> { serializer.Deserialize<T>(reader) };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }
    }
}
