#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

using BT = Common.Entity.BasicType;

namespace Common.Entity;
public abstract class ParsedEntity : IParsedEntity, IEquatable<IParsedEntity>, IEntity
{
  /// <summary>Gets or sets the parent entity.</summary>
  /// <remarks>This is <see langword="null"/> if the current entity is a root entity.</remarks>
  public IParsedEntity? Parent { get; set; }
  public virtual string? Origin { get; set; }
  /// <summary>This should be overridden by any inherited class.</summary>
  /// <remarks>This determines the class of the entity.</remarks>
  public abstract BT Type { get; }
  /// <summary>Gets the property collections.</summary>
  public virtual IDictionary<string, IList<IParsedEntity>> PropertyCollections { get; } = new Dictionary<string, IList<IParsedEntity>>();
  /// <summary>Gets the child entities.</summary>
  public virtual IList<IParsedEntity> Children { get; } = [];
  /// <summary>Gets the property values.</summary>
  public virtual IDictionary<string, IParsedEntity> PropertyValues { get; } = new Dictionary<string, IParsedEntity>();
  /// <summary>Gets the data values.</summary>
  public virtual IDictionary<string, object?> DataValues { get; } = new Dictionary<string, object?>();

  public abstract bool Equals (IParsedEntity? other);
  public abstract override string? ToString ();
  public void SetParent (IParsedEntity parent) => Parent = parent;
}
public class ErrorEntity : ParsedEntity
{
  public required string Message { get; init; }
  public override BT Type => BT.Invalid;
  public override bool Equals (IParsedEntity? other) => false; // Error entities are never equal to anything else, even other error entities.
  public override string ToString () => $"Error: {Message}";
}
public class StringEntity : ParsedEntity, IPrimitiveEntity
{
  /// <summary>Gets the serialized representation of the string value.</summary>
  /// <remarks>This contains quotes.</remarks>
  public string Content => $"\"{Value}\"";
  /// <summary>Gets or sets the string value.</summary>
  public required string Value
  {
    get => (string) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }

  public override BT Type => BT.String;
  public static StringEntity CreateFrom (Match match) => new()
  {
    Value = match.Groups["value"].Value,
    Origin = match.Value,
  };
  public override bool Equals (IParsedEntity? other) =>
    other is StringEntity entity && Value.Equals(entity.Value, SCO);
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
    init => DataValues["Value"] = value;
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
  public override bool Equals (IParsedEntity? other) => other is NullEntity;
  public override string ToString () => NullString;
}
public class BooleanEntity : ParsedEntity, IPrimitiveEntity
{
  public required bool Value
  {
    get => (bool) DataValues["Value"]!;
    init => DataValues["Value"] = value;
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
    init => DataValues["Namespace"] = value;
  }
  public required string Key
  {
    get => (string) DataValues["Key"]!;
    init => DataValues["Key"] = value;
  }
  public required string Value
  {
    get => (string) DataValues["Value"]!;
    init => DataValues["Value"] = value;
  }

  public override bool Equals (IParsedEntity? other) =>
    other is AttributeEntity ae &&
    Key.Equals(ae.Key, SCO) &&
    Value.Equals(ae.Value, SCO) &&
    ((Namespace.IsEmpty && ae.Namespace.IsEmpty) || (Namespace?.Equals(ae.Namespace, SCO) == true));
  public override string ToString () => $"{Key}=\"{Value}\"";
}
public class PropertyEntity : ParsedEntity
{
  public override BT Type => BT.Property;
  public required string Key { get; init; }
  public required IParsedEntity Value { get; init; }

  public override bool Equals (IParsedEntity? other) =>
    other is PropertyEntity pe &&
    Key.Equals(pe.Key, SCO) &&
    Value.Equals(pe.Value);
  public override string ToString () => $"\"{Key}\":{Value}";
}
public class ElementEntity : ElementOpenPlaceholder
{
  public bool IsHeader { get; init; }
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
  public required string Name { get; init; }
  public string? Namespace { get; init; }
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
  public string? Namespace { get; init; }
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
    init => AddProperties(value);
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
    init => AddValues(value);
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
    init => DataValues["Content"] = value;
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
    init => DataValues["Content"] = value;
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
    init => DataValues["Name"] = value;
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
