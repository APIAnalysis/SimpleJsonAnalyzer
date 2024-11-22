Analyzer for comparing JSON and POCO types.

Especially useful for when deserializing fails.

Instead of error messages like:

```ascii
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

You can get useful human readable information like:


```ascii
- Expected 'OtherNumber' to be of type 'Byte' but server returned a 'Integer'.
- The JSON included the property 'Status' (with value '1') but it does not exist in class 'SimpleClass'.
- Expected 'SimpleClass' to have a property named 'Name' but it did not exist in JSON.
```

