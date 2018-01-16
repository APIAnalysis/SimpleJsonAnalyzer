# ApiAnalysisCollectionMayNotBeEmptyAttribute

After applying this attribute to a property that will be populated by a JSON Array, the JSON array must contain at least one element. If the JSON Array contains no element then the error message will be returned.

## Parameters

_none_

## Messages

The possible response message that may be returned is:

* UnexpectedEmptyCollectionMessage

## Example Usage

### C&#35;

```csharp
    [ApiAnalysisCollectionMayNotBeEmpty)]
    public string[] Items { get; set; }
```

### VB&#46;Net

```vbnet
    <ApiAnalysisCollectionMayNotBeEmpty>
    Public Property Items As List(Of String)
```
