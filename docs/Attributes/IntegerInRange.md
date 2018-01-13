# ApiAnalysisIntegerInRangeAttribute

After applying this attribute to an `Integer property, the JSON value must be within the range of the specified numbers (inclusive.) If the JSON value is outside of the range then an appropriate message will be returned.

## Parameters

To use this attribute, two integers must be passed to the constructor.  
The **first** is **lowest** acceptable value.  
The **second** is the **highest** acceptable value.

## Messages

The possible response messages that may be returned are:

* ValueWasLowerThanBoundaryMessage
* ValueWasHigherThanBoundaryMessage

## Example Usage

### C&#35;

```csharp
    [ApiAnalysisIntegerInRange(5, 10)]
    public int Number { get; set; }
```

### VB&#46;Net

```vbnet
    <ApiAnalysisIntegerInRange(5, 10)>
    Public Property Number As Integer
```
