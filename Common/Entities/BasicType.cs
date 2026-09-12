#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
/// <summary>The type of object.</summary>
public enum BasicType
{
  /// <summary>Invalid text. Unable to parse.</summary>
  Invalid = -1,
  /// <summary>This returns when you try to get a value that doesn't exist.</summary>
  Absent = 0,
  /// <summary>This will get removed by a later pass by the parser..</summary>
  Placeholder = 2,
  /// <summary>The single top level item from a data file, when <see cref="ParsingInfo.GeneratesSingleObject"/> is <see langword="true"/>.</summary>
  Document = 3,
  /// <summary>A comment item, no need to parse, ignore.</summary>
  Comment = 4,
  #region JSON
  /// <summary>The value 'null'.</summary>
  /// <remarks>JSON <see langword="null"/> value.</remarks>
  Null,
  /// <summary>Quoted text.</summary>
  /// <remarks>JSON values, JSON keys, XML Attribute Values, INI </remarks>
  String,
  /// <summary>Non-quoted numeric data.</summary>
  /// <remarks>JSON numeric values, not enclosed in quotes.</remarks>
  Number,
  /// <summary>An array of <see cref="IEntity"/> items.</summary>
  /// <remarks>A JSON array object, or a list of items.</remarks>
  Array,
  /// <summary>A basic dictionary.</summary>
  /// <remarks>
  /// A JSON object which can store named properties in an <see cref="IDictionary{TKey,TValue}"/>.
  /// </remarks>
  Object,
  /// <summary>A <see langword="true"/> or a <see langword="false"/> stored as 'true' and 'false'.</summary>
  Boolean,
  #endregion
  #region XML
  /// <summary>This is the starting and ending whitespace in an element, or whitespace between elements.</summary>
  IgnoredWhitespace,
  /// <summary>This is content within a mixed element.</summary>
  LooseContent,
  /// <summary>A complex object that stores attributes, elements, and content.</summary>
  Element,
  /// <summary>A key/value pair that can be qualified with a namespace.</summary>
  Attribute,
  #endregion
  #region INI / REG
  /// <summary>A named section.</summary>
  Section,
  /// <summary>A key\value pair in which the value is an entity.</summary>
  Property,
  #endregion INI / REG
  /// <summary>A custom type, for a different style structure than what has been defined already.</summary>
  Raw = 0x7fff,
  /// <summary>A custom class, for a different style structure than what has been defined already.</summary>
  /// <remarks>This class must inherit from <see cref="Entity"/>.</remarks>
  External = 0x8000,
  /// <summary>A constant character or set of characters.</summary>
  Operator = 0x8001,
  Omit = 0x8002
}
