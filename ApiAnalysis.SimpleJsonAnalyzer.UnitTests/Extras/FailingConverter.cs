// <copyright file="FailingConverter.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests.Extras;
#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar
public class FailingConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
#pragma warning restore RECS0083 // Shows NotImplementedException throws in the quick task bar