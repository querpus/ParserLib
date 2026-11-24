# ParserLib

Text, Binary, XML, & JSON Modular Parser.
Uses these objects primarily:

* `XParser` - The parser object. 
  * Instantiate one of these first to use the parser.
  * Be sure to assign a `Spec` and to pass it one of the following:
    *   a path as a string.
    *   text as a string.
    *   an `IEnumerable<byte>` like `Memory<byte>` or `byte[]`.
    *   an `XElement` from an XML document.
* `Spec` - The specification object, this contains the details about the process and methods for deconstructng a given language.
  * Must have the following Properties defined:
      * Name - `string`
      * Strategy - `OperationStrategy` (Enhanced `IList<IOperation>`)
      * TextFile - `bool` (Whether this spec loads a string or a byte array)
  * Optionally can define these to use more features:
      * TokenRules
      * GroupTokenRules
      * TokenRuleSet
      * NodeRuleSet
      * SC - `StringComparison` (A global forced case sensitivity setting for all operations)
* `Operation` - A task to perform on data stored in a key. It stores the result data in another key if any data is stored.
* `DataStore` - A dictionary of data keys and stored data from operations.

## Operation Reference

### Common Operations (* means operation may change)

* `ForCountOperation`*
* `IfOperation`*
* `SwitchOperation`*
* `BreakOperation`*
* `ContinueOperation`*
* `OperationStrategy`* (For reuse and grouping of multiple operation lists)

### Text Operations

* `TokenizeOperation`
* `TokenAssembleOperation`
* `ReplaceOperation`
* `SplitOperation`

### Binary Operations

* `ReadDataOperation`*
* TODO: Finish list
