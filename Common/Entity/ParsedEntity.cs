#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

using BT = Common.Entity.BasicType;

namespace Common.Entity;
public abstract class ParsedEntity : IParsedEntity, IEquatable<IParsedEntity>, IEntity, ITextSerializer
{
  /// <summary>Gets or sets the parent entity.</summary>
  /// <remarks>This is <see langword="null"/> if the current entity is a root entity.</remarks>
  public IParsedEntity? Parent { get; set; }
  public virtual string? Origin { get; set; }
  /// <summary>This should be overridden by any inherited class.</summary>
  /// <remarks>This determines the class of the entity.</remarks>
  public abstract BT Type { get; }
  /// <summary>Gets the property collections.</summary>
  /// <remarks>This is for collections of <see cref="IParsedEntity"/> objects.</remarks>
  public virtual Dictionary<string, IList<IParsedEntity>> PropertyCollections { get; } = [];
  /// <summary>Gets the child entities.</summary>
  public virtual IList<IParsedEntity> Children { get; } = [];
  /// <summary>Gets the property values.</summary>
  public virtual Dictionary<string, IParsedEntity> PropertyValues { get; } = [];
  /// <summary>Gets the data values.</summary>
  /// <remarks>This is for data that never gets assigned to a <see cref="IParsedEntity"/> object, or data that is associated with the entity.</remarks>
  public virtual Dictionary<string, object?> DataValues { get; } = [];

  public virtual bool Equals (IParsedEntity? other) =>
    other is PlaceholderEntity cust &&
    PropertyCollections.SequenceEqual(cust.PropertyCollections) &&
    PropertyValues.SequenceEqual(cust.PropertyValues) &&
    DataValues.SequenceEqual(cust.DataValues) &&
    Children.SequenceEqual(cust.Children);
  public abstract override string? ToString ();
  public void SetParent (IParsedEntity parent) => Parent = parent;
  public abstract string Serialize ();
}
public class ErrorEntity : ParsedEntity
{
  public required string Message { get; init; }
  /// <summary>Comment start.</summary>
  public string Prefix { get; init; } = SE;
  /// <summary>Comment End.</summary>
  public string Suffix { get; init; } = SE;
  public override BT Type => BT.Invalid;
  /// <summary>Error entities are never equal to anything else, even other error entities.</summary>
  /// <param name="other">The other entity to compare to.</param>
  /// <returns>Always returns <see langword="false"/>.</returns>
  public override bool Equals (IParsedEntity? other) => false;
  public override string Serialize () => $"{Prefix} {this} {Suffix}";
  public override string ToString () => $"Error: {Message}";
}
public class StringEntity() : ParsedEntity, IPrimitiveEntity
{
  public string Content => $"{Quote}{Value}{Quote}";
  /// <summary>Gets or sets the string value.</summary>
  public required dynamic Value
  {
    get => DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public string? Quote
  {
    get => (string) DataValues["Quote"]!;
    set => DataValues["Quote"] = value;
  }

  public override BT Type => BT.String;
  public static StringEntity CreateFrom (Match match) => new()
  {
    Value = match.Groups["value"].Value,
    Quote = match.Groups["quote"].Value,
    Origin = match.Value,
  };
  public override bool Equals (IParsedEntity? other) =>
    other is StringEntity entity && Value.Equals(entity.Value, SCO);
  /// <summary>Gets the serialized representation of the string value.</summary>
  /// <remarks>This contains quotes.</remarks>
  public override string Serialize () => throw new NotImplementedException();
  public override string? ToString () => Content;
}
/// <summary>An entity representing a document.</summary>
public class DocumentEntity : ParsedEntity
{
  /// <summary>The entire text contents of the file.</summary>
  public required string Content { get; init; }

  public IParsedEntity? RootNode
  {
    get => PropertyValues.TryGetValue("RootNode", out IParsedEntity? value) ? value : null;
    set => PropertyValues["RootNode"] = value!;
  }
  public IParsedEntity? Header
  {
    get => PropertyValues.TryGetValue("Header", out IParsedEntity? value) ? value : null;
    set => PropertyValues["Header"] = value!;
  }
  public override BT Type => BT.Document;

  public override bool Equals (IParsedEntity? other) =>
    other is DocumentEntity je && Content.Equals(je.Content, SCO);
  public override string ToString () => Content;
  public void SetRoot (IParsedEntity? root)
  {
    if (root is null) return;
    RootNode = root;
    root.SetParent(this);
  }
  public void SetHeader (IParsedEntity? header)
  {
    if (header is null)
      return;
    Header = header;
    header.SetParent(this);
  }
}
/// <summary>An entity representing a number or decimal.</summary>
public class NumberEntity : ParsedEntity, IPrimitiveEntity
{
  public required decimal Value
  {
    get => (decimal) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public override BT Type => BT.Number;

  public string Content => $"{Value}";

  public override bool Equals (IParsedEntity? other) => other is NumberEntity ne && ne.Value == Value;
  public override string ToString () => $"{Value}";
}
/// <summary>An entity representing a null value.</summary>
public class NullEntity : ParsedEntity, IPrimitiveEntity
{
  private const string NullString = "null";

  public override BT Type => BT.Null;

  public string Content => NullString;
  string IPrimitiveEntity.Value => NullString;
  public object? Value => null;
  public override bool Equals (IParsedEntity? other) => other is NullEntity;
  public override string ToString () => NullString;
}
public class BooleanEntity : ParsedEntity, IPrimitiveEntity
{
  public required bool Value
  {
    get => (bool) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public override BT Type => BT.Boolean;
  public string Content => Value ? bool.TrueString : bool.FalseString;
  public override bool Equals (IParsedEntity? other) => other is BooleanEntity be && be.Value == Value;
  public override string ToString () => Content;
}
public class AttributeEntity : ParsedEntity
{
  public override BT Type => BT.Attribute;
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public required string Key
  {
    get => (string) DataValues["Key"]!;
    set => DataValues["Key"] = value;
  }
  public required string Value
  {
    get => (string) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }

  public override bool Equals (IParsedEntity? other) =>
    other is AttributeEntity ae &&
    Key.Equals(ae.Key, SCO) &&
    Value.Equals(ae.Value, SCO) &&
    ((Namespace.IsEmpty && ae.Namespace.IsEmpty) || (Namespace?.Equals(ae.Namespace, SCO) == true));
  public override string ToString () => $"{(Namespace.IsEmpty ? "": $"{Namespace}:")}{Key}=\"{Value}\"";
}
public class PropertyEntity : ParsedEntity
{
  public override BT Type => BT.Property;
  public required string Key { get; set; }
  public required IParsedEntity Value { get; set; }

  public override bool Equals (IParsedEntity? other) =>
    other is PropertyEntity pe &&
    Key.Equals(pe.Key, SCO) &&
    Value.Equals(pe.Value);
  public override string ToString () => $"\"{Key}\":{Value}";
}
public class ElementEntity : ElementOpenPlaceholder
{
  public bool IsHeader { get; set; }
  public override BT Type => BT.Element;

  public override string ToString ()
  {
    string attrs = Attributes.Select(child => child.ToString()).TextJoin(" ");
    string children = Children.Select(child => child.ToString()).TextJoin(Chars.LFs);

    if (IsHeader)
      return $"<?xml {attrs}?>";

    string elem = $"<{Name} {attrs}";

    return Children.OfType<ElementEntity>().ICount == 0
    ? elem + " />"
    : elem + ">" + children + $"</{Name}>";
  }
  public void AddChild (IParsedEntity child)
  {
    child.SetParent(this);
    Children.Add(child);
  }
  public void AddChildren (IEnumerable<IParsedEntity> children) => children.Foreach(AddChild);
  public override bool Equals (IParsedEntity? other) =>
    other is ElementEntity ee &&
    Attributes.SequenceEqual(ee.Attributes) &&
    Children.SequenceEqual(ee.Children) &&
    Name.Equals(ee.Name, SCO);
}
public class ElementClosePlaceholder : ParsedEntity
{
  public required string Name { get; set; }
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public override BT Type => BT.Placeholder;

  public override bool Equals (IParsedEntity? other) =>
    other is ElementClosePlaceholder ecp &&
    Name.Is(ecp.Name) &&
    ((Namespace is null && ecp.Namespace is null) || Namespace.Is(ecp.Namespace));
  public override string ToString () => $"</{Name}>";
}
public class ElementOpenPlaceholder : ParsedEntity
{
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    init => DataValues["Name"] = value;
  }

  public Collection<IParsedEntity> Attributes
  {
    get => (Collection<IParsedEntity>) PropertyCollections["Attributes"];
    init => AddAttributes(value);
  }
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public override BT Type => BT.Placeholder;
  public void AddAttribute (IParsedEntity attribute)
  {
    attribute.SetParent(this);
    Attributes.Add(attribute);
  }
  public void AddAttributes (IEnumerable<IParsedEntity> attributes) => attributes.Foreach(AddAttribute);
  public override bool Equals (IParsedEntity? other) =>
    other is ElementOpenPlaceholder eop &&
    Name.Is(eop.Name) &&
    Attributes.SequenceEqual(eop.Attributes) &&
    ((Namespace is null && eop.Namespace is null) || Namespace.Is(eop.Namespace));
  public override string ToString () => $"</{Name}>";

}
public class ContentEntity : ParsedEntity, IPrimitiveEntity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    init => DataValues["Content"] = value;
  }
  public override BT Type => BT.LooseContent;

  public override bool Equals (IParsedEntity? other) =>
    other is ContentEntity ce && Content.Is(ce.Content);
  public override string ToString () => Content;
}
public class CommentEntity : ContentEntity
{
  public override BT Type => BT.Comment;
  public override bool Equals (IParsedEntity? other) =>
    other is CommentEntity ce && Content.Is(ce.Content);
}
public class ObjectEntity : ParsedEntity
{
  public Collection<IParsedEntity> Properties
  {
    get => (Collection<IParsedEntity>) PropertyCollections["Properties"];
    set => AddProperties(value);
  }
  public override BT Type => BT.Object;

  public void AddProperty (IParsedEntity property)
  {
    property.SetParent(this);
    PropertyCollections["Properties"].Add(property);
  }

  public void AddProperties (IEnumerable<IParsedEntity> properties) => properties.Foreach(AddProperty);
  public override bool Equals (IParsedEntity? other) =>
    other is ObjectEntity oe && Properties.SequenceEqual(oe.Properties);
  public override string ToString () => Properties.TextJoin(",");
}
public class ArrayEntity : ParsedEntity
{
  public void AddValue (IParsedEntity child)
  {
    child.SetParent(this);
    PropertyCollections["Values"].Add(child);
  }
  public void AddValues (IEnumerable<IParsedEntity> children) => children.Foreach(AddValue);
  public Collection<IParsedEntity> Values
  {
    get => (Collection<IParsedEntity>) PropertyCollections["Values"];
    set => AddValues(value);
  }
  public override BT Type => BT.Array;

  public override bool Equals (IParsedEntity? other) =>
    other is ArrayEntity ce && Values.SequenceEqual(ce.Values);
  public override string ToString () => Values.TextJoin(",");
}
public class SymbolEntity : ParsedEntity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    set => DataValues["Content"] = value;
  }
  public override BT Type => BT.Placeholder;

  public static implicit operator string (SymbolEntity ce) => ce.Content;
  public static implicit operator SymbolEntity (string s) => new()
  {
    Content = s,
    Origin = s
  };
  public static bool operator == (SymbolEntity left, string right) => left.Content.Is(right);
  public static bool operator != (SymbolEntity left, string right) => !(left == right);

  public override bool Equals (IParsedEntity? other) =>
    other is SymbolEntity ce && Content.Is(ce.Content);
  public override string ToString () => Content;

  public override int GetHashCode () => Content.GetHashCode(SCO);
  public override bool Equals (object? obj) => obj switch
  {
    string s => this == s,
    IParsedEntity ipe => Equals(ipe),
    _ => false
  };
}
public class WhitespaceEntity : ParsedEntity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    set => DataValues["Content"] = value;
  }
  public override BT Type => BT.IgnoredWhitespace;

  public override bool Equals (IParsedEntity? other) =>
    other is ContentEntity ce && Content.Is(ce.Content);
  public override string ToString () => Content;
}
public class SectionEntity : ParsedEntity
{
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    set => DataValues["Name"] = value;
  }
  public override BT Type => BT.Section;

  public override bool Equals (IParsedEntity? other) =>
    other is SectionEntity ce && Name.Is(ce.Name) && Properties.SequenceEqual(ce.Properties);
  public override string ToString () => $"[{Name}]" + '\n' + Properties.TextJoin("\n");
  public Dictionary<string, IParsedEntity> Properties => (Dictionary<string, IParsedEntity>) PropertyCollections["Properties"];
}
public class CustomEntity : ParsedEntity
{
  public override BT Type => BT.Custom;

  public override bool Equals (IParsedEntity? other) =>
    other is CustomEntity cust &&
    PropertyCollections.SequenceEqual(cust.PropertyCollections) &&
    PropertyValues.SequenceEqual(cust.PropertyValues) &&
    DataValues.SequenceEqual(cust.DataValues) &&
    Children.SequenceEqual(cust.Children);
  public override string? ToString () => "CustomEntity Data:" + DataValues.TextJoin(",") + " | Children: " + Children.TextJoin(",");
}

public class PlaceholderEntity : ParsedEntity
{
  public override BT Type => BT.Placeholder;

  public required Match Match
  {
    get => (Match) DataValues["Match"]!;
    set => DataValues["Match"] = value;
  }
  public override string? ToString () => "CustomEntity Data:" + DataValues.TextJoin(",") + " | Children: " + Children.TextJoin(",");
}
