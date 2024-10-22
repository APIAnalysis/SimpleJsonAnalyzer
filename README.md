# SimpleJsonAnalyzer

A simplified version of the JsonAnalyzer. Great for validating local JSON files or when you already have JSON in a string.

[![Build status](https://ci.appveyor.com/api/projects/status/qmbiy8arfl9fxbcl?svg=true)](https://ci.appveyor.com/project/mrlacey/simplejsonanalyzer)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
![Install from NuGet](https://img.shields.io/nuget/v/ApiAnalysis.SimpleJsonAnalyzer)

## Why is it needed?

- Just because something can be deserialized doesn't mean it's right.
- Deserialization failure messages aren't good enough.
- JSON and POCO types have some fundamental differences [de]serialization ignores.
- Because validation rules for a type should be kept with that type, not in a separate location.

## What's it for?

If you need to track that JSON you receive hasn't changed format, doesn't have content that might cause you issues in the near future, or if you need to verify correctness of a piece of JSON and other approaches aren't suitable.

## How does it work?

The analyzer compares a type with a piece of JSON and reports any differences.
If the JSON can't be deserialized it will give you more information. It can also provide additional details when something can be deserialized.

### Here's a very simple example

Consider this class.

```csharp
    public class SimpleClass
    {
        public byte Number { get; set; }
        public byte OtherNumber { get; set; }
        public string Name { get; set; }
    }
```

Now imagine you wanted to deserialize this string to that type.

```csharp
var jsonString = "{\"Number\":2, \"OtherNumber\":23445,\"Status\":1}";
```

You could just deserialize it:

```csharp
Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleClass>(jsonString);
```

but that will give you a lot of information when it fails, but only about one of the issues.

```
JsonSerializationException: Error converting value 23445 to type 'System.Byte'. Path 'OtherNumber', line 1, position 32.
at object Newtonsoft.Json.Serialization.JsonSerializerInternalReader.EnsureType (JsonReader reader, object value, CultureInfo culture, JsonContract contract, Type targetType)
at bool Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue (JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target)
at object Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject (object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, string id)
at object Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateObject (JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
at object Newtonsoft.Json.Serialization.JsonSerializerInternalReader.Deserialize (JsonReader reader, Type objectType, bool checkAdditionalContent)
at object Newtonsoft.Json.JsonSerializer.DeserializeInternal (JsonReader reader, Type objectType)
at object Newtonsoft.Json.JsonConvert.DeserializeObject (string value, Type type, JsonSerializerSettings settings)
at T Newtonsoft.Json.JsonConvert.DeserializeObject<T> (string value, JsonSerializerSettings settings)
at void <<Initialize>>d__0.MoveNext ()
at void System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw ()
at void System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification (Task task)
at void Xamarin.Interactive.Compilation.CompilationExecutionContext<TScriptResult>.<RunAsync>d__12.MoveNext ()
at void System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw ()
at void System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification (Task task)
at void Xamarin.Interactive.Repl.ReplExecutionContext.<RunAsync>d__9.MoveNext ()

▶ OverflowException: Value was either too large or too small for an unsigned byte.
```

However, SimpleJsonAnalyzer can tell you more about the differences.

```csharp
    var analyzer = new ApiAnalysis.SimpleJsonAnalyzer();
    var results = await analyzer.AnalyzeJsonAsync(jsonString, typeof(SimpleClass));

    foreach (var result in results)
    {
        Console.WriteLine(result);
    }
```

This tells us three things:

```
- Expected 'OtherNumber' to be of type 'Byte' but server returned a 'Integer'.
- The JSON included the property 'Status' (with value '1') but it does not exist in class 'SimpleClass'.
- Expected 'SimpleClass' to have a property named 'Name' but it did not exist in JSON.
```

This is helpful because:

- It's more easily readable.
- It's more complete in it's output.
- It can be run even when JSON can be deserialized without issue.
- It can provide additional information when a deserialization error occurs.

more docs to follow...

Powered, in part, by [JSON.Net](https://www.newtonsoft.com/json).
