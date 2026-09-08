#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entity.BasicType;

namespace Common.Entity;
public class ParsingSet
{
  public GlobalParsingOptions Global { get; init; } = new GlobalParsingOptions()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    TotalPasses = 2
  };
  public IImmutableList<TokenParsingOptions> TokenOptions { get; set; } = [];
  public IImmutableList<EntityParsingOptions> EntityOptions { get; init; } = [];
}

public static class DefaultParsingSets
{
  public static ParsingSet XML { get; } = new ParsingSet()
  {
    Global = new GlobalParsingOptions()
    {
      GeneratesSingleObject = true,
      IgnoreCase = false,
      TotalPasses = 2
    },
    EntityOptions = [
    new() {
      IndicatedItem = new() { Group = "header" },
      Type = BT.Element,
    }, new() {
      IndicatedItem = new() { Group = "header" },
      Type = BT.Element,
    }, new() {
      CreateEmptyAtStart = true,
      Type = BT.Document,
      OnlyAtTopLevel = true,
      SetAsNextLevelParent = true,
    },new() {
      IndicatedItem = new() { Group = "ws" },
      Type = BT.IgnoredWhitespace,
    }, new() {
       IndicatedItem = new() { Group = "content" },
       Type = BT.LooseContent,
       StoresData = true,

    }, new() {
       IndicatedItem = new() { Group = "ws" },
       Type = BT.IgnoredWhitespace,
    }, new() {
       IndicatedItem = new() { Group = "ws" },
       Type = BT.IgnoredWhitespace,
    }, new() {
    }]
  };

}
public sealed class ParserContext
{
  public string? OriginText { get; set; }
  public IParsedEntity? Document { get; set; }
  public IParsedEntity? Parent { get; set; }
  public string? Key { get; set; }
  public int Depth { get; set; }
  public int Pass { get; set; }
  public T? CastParent<T> () where T : IParsedEntity
  {
    dynamic? parent = Parent;
    return (T?) parent;
  }
}
public struct GlobalParsingOptions : IEquatable<GlobalParsingOptions>
{
  /// <summary>The kind of object the end result of the operations should be.</summary>
  /// <remarks> This is for validation purposes.<br/>
  /// If <see langword="true"/>,The parser will make a single <see cref="IParsedEntity"/> object from the data.<br/>
  /// If <see langword="false"/>, the parser will make a <see cref="Collection{T}"/> of <see cref="IParsedEntity"/> objects.</remarks>
  public bool GeneratesSingleObject { get; set; }
  /// <summary>The number of iteritive loops the parser must go through.</summary>
  public int TotalPasses { get; set; }
  /// <summary>Whether to ignore case on non-regex matches.</summary>
  public bool IgnoreCase { get; set; }
  public readonly bool Equals (GlobalParsingOptions other) =>
    GeneratesSingleObject == other.GeneratesSingleObject &&
    TotalPasses == other.TotalPasses &&
    IgnoreCase == other.IgnoreCase;
  public override readonly bool Equals (object? obj) =>
    obj is GlobalParsingOptions other && Equals(other);
  public override readonly int GetHashCode () =>
    HashCode.Combine(GeneratesSingleObject, TotalPasses, IgnoreCase);
  public static bool operator == (GlobalParsingOptions left, GlobalParsingOptions right) => left.Equals(right);
  public static bool operator != (GlobalParsingOptions left, GlobalParsingOptions right) => !(left == right);
}

public class JSONParsingOptions
{
  public GlobalParsingOptions Global { get; } = new GlobalParsingOptions()
  {

    GeneratesSingleObject = true,
    IgnoreCase = false,
    TotalPasses = 2
  };
  public IImmutableList<TokenParsingOptions> TokenOptions { get; set; } = [];
  public IDictionary<IndicatedItem, EntityParsingOptions> EntityOptionsByIndicatedItem =>
    EntityOptions.ToDictionary(static e => e.IndicatedItem);

  public IImmutableList<EntityParsingOptions> EntityOptions { get; } = [
    new() {
      IndicatedItem = new() { Group = "key" },
      StoresData = true,
      Type = BT.Property,
      StoreAsPieceTypes = [new() { GroupName = "keyname", PropertyKey = "Value" }],
      SetAsNextLevelParent = true,
      SetPropKey = true
    }, new() {
      IndicatedItem = new() { Group = "strvalue" },
      StoresData = true,
      Type = BT.String,
      StoreAsPieceTypes = [new() { GroupName = "value", PropertyKey = "Value" }],
    }, new() {
      IndicatedItem = new() { Group = "boolvalue" },
      StoresData = true,
      Type = BT.Boolean,
      StoreAsPieceTypes = [new() { GroupName = "value", PropertyKey = "Value" }],
    }, new () {
      IndicatedItem = new() { Group = "numvalue" },
      StoresData = true,
      Type = BT.Number,
      StoreAsPieceTypes = [new() { GroupName = "value", PropertyKey = "Value" }],
    }, new() {
      IndicatedItem = new() { Group = "nullvalue" },
      Type = BT.Null,
      StoreAsPieceTypes = [new() { GroupName = "value", PropertyKey = "Value" }],
    }, new() {
      IndicatedItem = new() { Group = "comment" },
      StoresData = false,
      Type = BT.Comment,
    }, new() {
      IndicatedItem = new() { Group = "ws" },
      StoresData = false,
      Type = BT.IgnoredWhitespace,
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "{" },
      DepthChange = 1,
      Type = BT.Operator,
      ConstantValue = "{",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "}" },
      DepthChange = -1,
      Type = BT.Operator,
      ConstantValue = "}",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "[" },
      DepthChange = 1,
      Type = BT.Operator,
      ConstantValue = "[",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = ":" },
      Type = BT.Operator,
      ConstantValue = ":",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "," },
      DepthChange = -1,
      Type = BT.Operator,
      ConstantValue = ",",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "]" },
      DepthChange = -1,
      Type = BT.Operator,
      ConstantValue = "]",
    }];
}
public struct TokenParsingOptions
{
  public BT MakeType { get; set; }
}

/// <summary>Represents matching criteria for an indicated item, including an optional token type, a required capture group, an
/// optional exact value, and whether exact-value comparison ignores case.</summary>
/// <remarks>TokenType may be null if unspecified. Group must be present and have length > 0 for a match.
/// ExactValue may be null; when specified, the capture's value must equal ExactValue. IgnoreCase controls case
/// sensitivity when ExactValue is compared.</remarks>
public struct IndicatedItem : IEquatable<IndicatedItem>
{
  /// <summary>Gets or sets the token type, for example 'Bearer'.</summary>
  /// <remarks>May be null if the token type is unspecified.</remarks>
  public string? TokenType { get; set; }
  /// <summary>The group that must be present and have a length > 0.</summary>
  public string? Group { get; set; }
  /// <summary>Exact string value to match.</summary>
  public string? ExactValue { get; set; }
  /// <summary>if <see langword="true"/>, it ignores case on the exact value matching.</summary>
  public bool IgnoreCase { get; set; }

  public override readonly bool Equals (object? obj) => obj is IndicatedItem item && Equals(item);
  public readonly bool Equals (IndicatedItem other) => TokenType == other.TokenType && Group == other.Group && ExactValue == other.ExactValue && IgnoreCase == other.IgnoreCase;
  public readonly override int GetHashCode () => HashCode.Combine(TokenType, Group, ExactValue, IgnoreCase);
  public readonly bool Matches (Match match) =>
    (Group is null || match.Groups[Group].Success) &&
    (ExactValue is null || match.Value.Is(ExactValue));
  public static bool operator == (IndicatedItem left, IndicatedItem right) => left.Equals(right);
  public static bool operator != (IndicatedItem left, IndicatedItem right) => !(left == right);
}

public struct EntityParsingOptions : IEquatable<EntityParsingOptions>
{

  #region Functional Properties
  /// <summary>
  /// Can be any predefined type, or it can be the special value <see cref="BT.Custom"/>.
  /// This determines the class of entity that is produced.
  /// </summary>
  public BT Type { get; set; }
  /// <summary>The conditions that must be present for this entity to be produced.</summary>
  public IndicatedItem IndicatedItem { get; set; }
  /// <summary>The change in depth this token indicates.</summary>
  /// <remarks>
  /// Normally 0, 1, or -1.<br/>
  /// <c> 1</c>: Increase depth (descend). For example, an opening bracket '{'.<br/>
  /// <c> 0</c>: No change in depth. A comma ',' or a keyword in a statement. This is the default value.<br/>
  /// <c>-1</c>: Decrease depth (ascend). For example, a closing bracket '}'.
  /// </remarks>
  public int DepthChange { get; set; }
  public bool SetPropKey { get; set; }
  /// <summary>
  /// Adds this token to the parent stack, meaning it will recieve all tokens that are passed as data once the depth descends.<br/>
  /// This does not have to be the depth changing token.
  /// </summary>
  public bool SetAsNextLevelParent { get; set; }
  /// <summary>Creates this entity outside of the parsing loop as the initial container for all other tokens.</summary>
  public bool CreateEmptyAtStart { get; set; }
  #endregion
  #region Data Properties
  /// <summary>The constant value if this entity always has the same value.</summary>
  public string? ConstantValue { get; set; }
  /// <summary>The type of piece this entity stores as.</summary>
  public ImmutableList<(string GroupName, string PropertyKey)> StoreAsPieceTypes { get; set; }
  #endregion Data Properties
  #region Informative Properties
  /// <summary>Whether or not the token stores data into its parent.</summary>
  public bool StoresData { get; set; }
  /// <summary>Whether or not this token causes an structural change to parsing.</summary>
  public bool DefinesStructure { get; set; }
  #endregion
  #region Validation Properties
  /// <summary>Only allow this entity at top-level, not as a child.</summary>
  public bool OnlyAtTopLevel { get; set; }

  public override bool Equals (object? obj) => obj is EntityParsingOptions other && Equals(other);
  public override readonly int GetHashCode () => HashCode.Combine(Type, IndicatedItem, DepthChange, SetPropKey, SetAsNextLevelParent, CreateEmptyAtStart, ConstantValue, StoreAsPieceTypes, HashCode.Combine(StoresData, DefinesStructure, OnlyAtTopLevel));

  public static bool operator == (EntityParsingOptions left, EntityParsingOptions right) =>
    left.Equals(right);

  public static bool operator != (EntityParsingOptions left, EntityParsingOptions right) => !(left == right);
  public readonly bool Equals (EntityParsingOptions other) => GetHashCode() == other.GetHashCode();
  #endregion
}

/// <summary>The type of object.</summary>
public enum BasicType
{
  /// <summary>Invalid text. Unable to parse.</summary>
  Invalid = -1,
  /// <summary>This returns when you try to get a value that doesn't exist.</summary>
  Absent = 0,
  /// <summary>This gets removed by the second stage parser.</summary>
  Placeholder = 2,
  /// <summary>The top level item from a data file, or when <see cref="GlobalParsingOptions.GeneratesSingleObject"/> is <see langword="true"/>.</summary>
  Document = 3,
  Comment = 4,
  #region JSON
  /// <summary>The value 'null'.</summary>
  /// <remarks>JSON <see langword="null"/> value.</remarks>
  Null,
  /// <summary>Quoted text.</summary>
  /// <remarks>JSON values, JSON keys, XML Attribute Values, INI </remarks>
  String,
  /// <summary>Non-quoted numeric data.</summary>
  Number,
  /// <summary>An array of <see cref="IParsedEntity"/> items.</summary>
  Array,
  /// <summary>A basic dictionary.</summary>
  Object,
  /// <summary>A <see langword="true"/> or a <see langword="false"/> stored as 'true' and 'false'.</summary>
  Boolean,
  #endregion
  #region XML
  /// <summary>This is the starting and ending whitespace in an element.</summary>
  IgnoredWhitespace,
  /// <summary>This is content within a mixed element.</summary>
  LooseContent,
  /// <summary>A complex object that stores attributes, elements, and content.</summary>
  Element,
  Attribute,
  #endregion
  #region INI / REG
  /// <summary>A named section.</summary>
  Section,
  /// <summary>A Key\Value pair.</summary>
  Property,
  #endregion INI / REG
  /// <summary>The type of a custom type, for a different style structure than what has been defined already.</summary>
  Custom = 0x7fff,
  /// <summary>A constant character or set of characters.</summary>
  Operator = 0x8000
}
public static class BasicTypeExt
{
  extension(BT type)
  {
    /// <summary>This type is a primitive value.</summary>
    public bool IsPrimitive => type is BT.Number or BT.String or BT.Boolean or BT.Null;
    public bool IsDictionary => type is BT.Object or BT.Element or BT.Section;
    public bool IsCollection => type is BT.Array or BT.Element or BT.Object or BT.Document;
  }
}
