#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;
using System.Xml.Linq;

using BT = Common.Entity.BasicType;

namespace Common.Entity;
public abstract class ParsedEntity : IParsedEntity, IEquatable<IParsedEntity>, IEntity
{
  /// <summary>Gets or sets the parent entity.</summary>
  /// <remarks>This is <see langword="null"/> if the current entity is a root entity.</remarks>
  public IParsedEntity? Parent { get; set; }
  public virtual string? Origin { get; init; }
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
/// <summary>An entity representing an xml document.</summary>
public class XMLDocumentEntity : JSONDocumentEntity
{
  public IParsedEntity? Header { get; protected set; }
  public IParsedEntity? RootNode { get; protected set; }
  public override BT Type => BT.Document;

  public override bool Equals (IParsedEntity? other) =>
    other is XMLDocumentEntity de && Content.Equals(de.Content, SCO);
  public override string ToString () => Content;
  public void SetHeader (IParsedEntity? header) => Header = header;
}
/// <summary>An entity representing a json document.</summary>
public class JSONDocumentEntity : ParsedEntity
{
  /// <summary>The entire text contents of the file.</summary>
  public required string Content { get; init; }
  public IParsedEntity? RootObject { get; protected set; }
  public override BT Type => BT.Document;

  public override bool Equals (IParsedEntity? other) =>
    other is JSONDocumentEntity je && Content.Equals(je.Content, SCO);
  public override string ToString () => Content;
  public void SetRoot (IParsedEntity root)
  {
    RootObject = root;
    root.SetParent(this);
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

    return Children.Count == 0
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
public static partial class EntityFactory
{
  #region JSON Regex
  /// <summary>JSON Tokenizing Regex</summary>
  [GeneratedRegex(JSONRegex, ROIPW | ROML | ROEC, 3000)]
  [AllowNull]
  private static partial Regex JSON_PreCompiled { get; }
  [SS("regex")]
  private const string AfterKey = @"(?<=[:=]\s*)";
  [SS("regex")]
  private const string JSONRegex =
    $$$"""
    (?#primitives)
    (?'key'        " (?'keyname'\w+) " (?=\s*[:=])) |
    (?'strvalue'   {{{AfterKey}}} " (?'value'([^\\"]|\\.)*) " ) |
    (?'numvalue'   {{{AfterKey}}}   (?'value'[0-9.eExXbB]+ )  ) |
    (?'boolvalue'  {{{AfterKey}}}   (?'value'true|false)      ) |
    (?'nullvalue'  {{{AfterKey}}}   (?'value'null)            ) |
    (?#operators)
    (?'Op'            [[\]{},=:]) |
    (?#commments)
    (?'comment'        \/\/.* ) |
    (?'comment'        \/\*([^*]|\*[^/])*\*\/ ) |
    (?# Other Whitespace)
    (?'ws'             \s+)
    """;
  #endregion
  #region XML Regex
  [GeneratedRegex(XMLRegex, ROIPW | ROML | ROEC, 3000)]
  [AllowNull]
  private static partial Regex XML_PreCompiled { get; }
  [SS("regex")]
  private const string XMLRegex =
    """
    (?# Element Piece)
    (?'element'
      <
      (?# '?' for header definition)
      (?'header'\?)?  \s*
      (?'close'\/)?   \s*
      (?# optional namespace)
      ((?'ns'\w+):)?
      (?'name'\w+)

      (?# attributes)
      (   \s+ 
          (?'attribute'
          ((?'attrns'\w+)     \s*     :     \s*)?
          (?'attrname'\w+)    \s*     =     \s*
         "(?'attrval'(  [^\n"\\]  |  \\[^\n]  )*  )"
        ))*

      (?'single'\s*\/)?
      \s*
      (?# '?' for ending the header definition)
      (\k'header')?
      >
    ) |

    (?# Leading or Trailing Whitespace)
    (?'ws'(?<=\>)\s+) |
    (?'ws'(?<=[^\s>])\s+) |
    (?# Leading or Trailing Whitespace)
    (?'content'(?<=\>\s*)[^<]+?(?=\s*<)) |
    (?# XML Comment)
    (?'comment'<!-- ([^-]| -[^-])* -->)
    """;
  #endregion

  private static Collection<IParsedEntity> ParseAttributes (Match match)
  {
    Collection<string> origins = [.. match.Groups["attributes"].Captures.Select(c => c.Value)];
    Collection<string> namespaces = [..
      from o in origins
      let colon = o.IndexOf(':', SCO)
      select colon != DNE ? o[..colon] : SE];
    Collection<string> keys = [.. match.Groups["attrname"] .Captures.Select(c => c.Value)];
    Collection<string> values = [.. match.Groups["attrval"].Captures.Select(c => c.Value)];
    if (keys.Count == origins.Count && values.Count == origins.Count)
    {
      IEnumerable<((string Key, string Value, string Origin) First, string Namespace)> zip = keys.Zip(values, origins).Zip(namespaces);
      return [.. zip.Select(t => new AttributeEntity() { Key = t.First.Key, Value = t.First.Value, Origin = t.First.Origin, Namespace = t.Namespace })];
    }
    else
    {
      throw new InvalidOperationException($"Keys ({keys.Count}) and Values ({values.Count}) do not match Origin Count ({origins.Count}).");
    }
  }
  private static Collection<IParsedEntity> ParseAttributes (XElement element, ElementEntity parent)
  {
    Collection<IParsedEntity> result = [];
    foreach (XAttribute attr in element.Attributes())
    {
      result.Add(new AttributeEntity()
      {
        Key = attr.Name.LocalName,
        Value = attr.Value,
        Origin = attr.ToString(),
        Parent = parent,
        Namespace = attr.Name.NamespaceName.IsEmpty ? null : attr.Name.NamespaceName
      });
    }
    return result;
  }
  private static ElementEntity GetHeader (Match match) => new()
  {
    IsHeader = true,
    Name = "xml",
    Origin = match.Value,
    Attributes = [.. ParseAttributes(match)],
  };
  private static ContentEntity GetContent (Match match) => new()
  {
    Content = match.Value,
    Origin = match.Value
  };
  private static CommentEntity GetComment (Match match) => new()
  {
    Content = match.Value,
    Origin = match.Value
  };
  private static WhitespaceEntity GetWhitespace (Match match) => new()
  {
    Content = match.Value,
    Origin = match.Value
  };
  private static ElementClosePlaceholder GetClose (Match match) => new()
  {
    Name = match.Groups["name"].Value,
    Namespace = match.HasValidGroup("ns") ? match.Groups["ns"].Value : null,
    Origin = match.Value,
  };
  private static ElementOpenPlaceholder GetOpen (Match match) => new()
  {
    Name = match.Groups["name"].Value,
    Namespace = match.HasValidGroup("ns") ? match.Groups["ns"].Value : null,
    Origin = match.Value,
    Attributes = [.. ParseAttributes(match)]
  };
  private static ElementEntity GetElement (Match match) => new()
  {
    Name = match.Groups["name"].Value,
    Namespace = match.HasValidGroup("ns") ? match.Groups["ns"].Value : null,
    Origin = match.Value,
    Attributes = [.. ParseAttributes(match)]
  };
  private static StringEntity GetString (Match match) => new()
  {
    Value = match.Groups["value"].Value,
    Origin = match.Value
  };
  private static NumberEntity GetNumber (Match match) => new()
  {
    Value = decimal.TryParse(match.Groups["value"].Value, out decimal dec) ? dec : throw new InvalidValueException(match.Groups["value"].Value),
    Origin = match.Value
  };
  private static BooleanEntity GetBoolean (Match match) => new()
  {
    Value = bool.Parse(match.Groups["value"].Value),
    Origin = match.Value
  };
  private static NullEntity GetNull (Match match) => new()
  {
    Origin = match.Value
  };
  private static SymbolEntity GetSymbol (Match match) => new()
  {
    Content = match.Value,
    Origin = match.Value
  };
  private static IParsedEntity GetEntity (Match match, EntityParsingOptions options, ParserContext context)
  {
    if (!options.IndicatedItem.Matches(match))
    {
      return new ErrorEntity()
      {
        Message = $"The match did not meet the requirements for this entity. ({options.IndicatedItem})",
        Origin = match.Value
      };
    }

    return options.Type switch
    {
      _ when options.ConstantValue is not null => new SymbolEntity()
      {
        Content = options.ConstantValue,
        Origin = match.Value
      },
      BT.String => GetString(match),
      BT.Number => GetNumber(match),
      BT.Boolean => GetBoolean(match),
      BT.Null => GetNull(match),
      BT.Comment => GetComment(match),
      BT.IgnoredWhitespace => GetWhitespace(match),
      BT.Array => new ArrayEntity()
      {
        Origin = match.Value
      },
      BT.Object => new ObjectEntity()
      {
        Origin = match.Value
      },
      BT.Custom => new CustomEntity()
      {
        Origin = match.Value,
      },
      BT.Invalid => throw new InvalidOperationException("Type was Invalid."),
      BT.Absent => throw new InvalidOperationException("Type was Absent."),
      BT.Placeholder => throw new InvalidOperationException("Type was Placeholder."),
      BT.Document => new XMLDocumentEntity() { Content = match.Value, Origin = match.Value },
      BT.LooseContent => GetContent(match),
      BT.Element when match.HasValidGroup("name") => new ElementEntity()   { Origin = match.Value, Name = match.Groups["name"].Value },
      BT.Attribute when context.Key is string key => new AttributeEntity() { Origin = match.Value, Key = key, Value = match.Value, },
      BT.Section when match.HasValidGroup("name") => new SectionEntity()   { Origin = match.Value, Name = match.Groups["name"].Value },
      BT.Property when context.Key is string key  => new PropertyEntity()  { Origin = match.Value, Key = key, Value = GetString(match) },
      BT.Operator => GetSymbol(match),
      _ => throw new InvalidOperationException($"The entity type {options.Type} is not supported."),
    };
  }

  private static IParsedEntity ElementSelector (Match match)
  {
    if (match.HasValidGroup("header"))
      return GetHeader(match);

    if (match.HasValidGroup("close"))
      return GetClose(match);

    if (match.HasValidGroup("single"))
      return GetElement(match);

    return GetOpen(match);
  }
  private static IParsedEntity ValueSelector (Match match)
  {
    if (match.HasValidGroup("strvalue"))
      return GetString(match);

    if (match.HasValidGroup("numvalue"))
      return GetNumber(match);

    if (match.HasValidGroup("boolvalue"))
      return GetBoolean(match);

    if (match.HasValidGroup("nullvalue"))
      return GetNull(match);

    throw new InvalidOperationException("The internal value group needed to process this item is missing.");
  }
  private static IParsedEntity CheckXMLMatch (Match match)
  {
    if (!match.Success) throw new InvalidOperationException("Match was not a success.");

    foreach (EntityParsingOptions opts in DefaultParsingSets.XML.EntityOptions)
    {
      if (opts.IndicatedItem.Matches(match))
        return GetEntity(match, opts, new());
    }

    if (match.HasValidGroup("element"))
      return ElementSelector(match);

    if (match.HasValidGroup("content"))
      return GetContent(match);

    if (match.HasValidGroup("comment"))
      return GetComment(match);

    if (match.HasValidGroup("ws"))
      return GetWhitespace(match);

    throw new InvalidOperationException("The groups needed to process this item are missing.");
  }
  private static IParsedEntity CheckJSONMatch (Match match)
  {
    if (!match.Success) throw new InvalidOperationException("Match was not a success.");

    if (match.HasValidGroup("value"))
      return ValueSelector(match);

    if (match.HasValidGroup("key"))
      return GetContent(match);

    if (match.HasValidGroup("comment"))
      return GetComment(match);

    if (match.HasValidGroup("op"))
      return GetSymbol(match);

    if (match.HasValidGroup("ws"))
      return GetWhitespace(match);

    throw new InvalidOperationException("The groups needed to process this item are missing.");
  }

  public static IParsedEntity FromXElement (XElement root, ParserContext? context)
  {
    context ??= new() { OriginText = root.Value };
    XMLDocumentEntity document = new()
    {
      Origin = root.Value,
      Content = root.Value,
    };
    context.Document = document;

    ElementEntity parent = new()
    {
      Name = root.Name.LocalName,
      Origin = root.Value,
      Parent = context.Parent ?? document,
      Namespace = root.Name.NamespaceName.IsEmpty ? null : root.Name.NamespaceName,
    };
    context.Parent = parent;
    document.SetRoot(parent);

    parent.AddAttributes(ParseAttributes(root, parent));
    parent.AddChildren([.. root.Elements().Select(xe => FromXElement(xe, context))]);

    return document;
  }
  public static IParsedEntity JSONFromString (string content)
  {
    Collection<IParsedEntity> inside = [];
    Collection<string?> keys = [];
    IParsedEntity? parent = null;
    MatchCollection matches = JSON_PreCompiled.Matches(content);
    IParsedEntity? document = new JSONDocumentEntity()
    {
      Origin = content,
      Content = content,
    };

    int get_depth () => inside.Count - 1;

    IParsedEntity obj_create (IParsedEntity? inside_entity, IParsedEntity child_obj)
    {
      if (inside_entity is null)
        (document as JSONDocumentEntity)?.SetRoot(child_obj);
      else if (inside_entity is ObjectEntity oe)
        oe.AddProperty(child_obj);
      else if (inside_entity is ArrayEntity ae)
        ae.AddValue(child_obj);
      else
        throw new InvalidOperationException($"Cannot create an object inside a {inside_entity.Type}.");

      inside.Add(child_obj);
      keys.Add(null);
      return child_obj;
    }
    IParsedEntity? obj_exit () => keys[get_depth()] is null ? inside.Pop() :
      throw new InvalidOperationException($"Key {keys[get_depth()]} ws not popped.");
    void obj_set_key (string key)
    {
      keys[get_depth()] = keys[get_depth()] is null ? key
        : throw new InvalidOperationException($"Key is already set for this object. ({keys[get_depth()]})");
    }
    string obj_pop_key ()
    {
      if (keys[get_depth()] is null)
      {
        throw new InvalidOperationException($"Key is not set for this object at depth {get_depth()}.");
      }
      else
      {
        string result = keys[get_depth()]!;
        keys[get_depth()] = null;
        return result;
      }
    }
    bool obj_chk_key () => keys[get_depth()] is not null;

    foreach (Match match in matches)
    {
      IParsedEntity item = CheckJSONMatch(match);

      switch (item)
      {
        // Ignore comments
        case CommentEntity ce:
          continue;
        // Object start
        case SymbolEntity se when se == "{":
          parent = obj_create(parent, new ObjectEntity());
          continue;
        case SymbolEntity se when se == "}":
          parent = obj_exit();
          continue;
        case SymbolEntity se when se == "[":
          parent = obj_create(parent, new ArrayEntity());
          continue;
        case SymbolEntity se when se == "]":
          parent = obj_exit();
          continue;
        case SymbolEntity se when se.Content is "," or ":":
          continue;
        // Property Entities are built here
        case PropertyEntity:
        // Element Entities are not allowed in JSON
        case ElementEntity or ContentEntity or AttributeEntity:
          throw new InvalidDataException($"Cannot have an entity of this type ({item.TypeName}) in a JSON factory.");
        // The keyname is empty, and we have a string entity, so this is the key for the next property.
        case StringEntity se when parent is ObjectEntity oe && !obj_chk_key():
          obj_set_key(se.Value);
          continue;
        // The keyname is not empty, and we have a primitive entity, so this is the value for the current property.
        case IPrimitiveEntity ipe when parent is ObjectEntity oe && obj_chk_key():
          string keyname = obj_pop_key();
          PropertyEntity prop = new()
          {
            Key = keyname,
            Origin = $"\"{keyname}\":{ipe.Origin}",
            Value = ipe,
          };
          oe.AddProperty(prop);
          continue;
        // We are in an array and we have a primitive entity
        case IPrimitiveEntity ipe when parent is ArrayEntity ae:
          ae.AddValue(ipe);
          continue;
        default:
          throw new InvalidOperationException($"Unhandled Entity \"{item.Origin}\" sent to EntityFactory.");
      }
    }
    return document;
  }
  public static IParsedEntity XMLFromString (string content)
  {
    IParsedEntity? document;
    IParsedEntity? parent = null;
    Collection<IParsedEntity> inside = [];
    MatchCollection matches = XML_PreCompiled.Matches(content);

    foreach (var o in DefaultParsingSets.XML.EntityOptions)
    {
      if (o.CreateEmptyAtStart)
      {
        IParsedEntity empty = GetEntity(Match.Empty, o, new());
        if (empty is ElementEntity ee)
        {
          parent = ee;
          inside.Add(parent);
        }
      }
    }

    document = new XMLDocumentEntity()
    {
      Origin = content,
      Content = content,
    };

    foreach (Match match in matches)
    {
      IParsedEntity item = CheckXMLMatch(match);

      switch (item)
      {
        case ElementEntity ee when ee.IsHeader:
          ((XMLDocumentEntity) document).SetHeader(item);
          continue;
        case ElementOpenPlaceholder eop when parent is null:
          parent = new ElementEntity()
          {
            Name = eop.Name,
            Origin = eop.Origin,
            Namespace = eop.Namespace,
            Parent = document,
            Attributes = eop.Attributes,
          };
          ((XMLDocumentEntity) document).SetRoot(parent);
          inside.Add(parent);
          continue;
        case WhitespaceEntity when parent is null:
          continue;
        case ContentEntity when parent is null:
          throw new InvalidDataException("Cannot have loose content outside the root element.");
        case ElementOpenPlaceholder inner_eop when parent is not null:
          ElementEntity inner = new()
          {
            Name = inner_eop.Name,
            Origin = inner_eop.Origin,
            Namespace = inner_eop.Namespace,
            Parent = parent,
            Attributes = inner_eop.Attributes,
          };
          ((ElementEntity) parent).AddChild(inner);
          inside.Add(inner);
          parent = inner;
          continue;
        case ElementClosePlaceholder inner_ecp when parent is ElementEntity ee:
          if (!ee.Name.Is(inner_ecp.Name))
            throw new InvalidDataException($"Mismatched elements, or you missed a closing tag somewhere. ({ee.Name}) != ({inner_ecp.Name})");
          inside.Drop();
          parent = inside.Peek();
          continue;
        case ContentEntity inner_ce when parent is not null:
          inner_ce.SetParent(parent);
          (parent as ElementEntity)?.AddChild(inner_ce);
          continue;
        case ElementEntity inner_ee when parent is not null:
          inner_ee.SetParent(parent);
          (parent as ElementEntity)?.AddChild(inner_ee);
          continue;
        case NumberEntity or StringEntity or NullEntity or AttributeEntity:
          throw new InvalidDataException($"Cannot have an entity of this type ({item.TypeName}) in an XML factory.");
        default:
          throw new InvalidOperationException($"Item was not handled. ({item.Type}, {item.Origin}) ");
      }
    }
    return document;
  }
  public static IParsedEntity FromString (string content, BT type) => type switch
  {
    BT.Null => new NullEntity(),
    BT.Element => XMLFromString(content),
    BT.Object => JSONFromString(content),
    _ => throw new InvalidOperationException($"Invalid BasicType ({type}) sent to EntityFactory."),
  };
}
