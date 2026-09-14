#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;
using System.Xml.Linq;

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class EntityFactory
{
  private static Collection<AttributeEntity> ParseAttributes (Match match)
  {
    Collection<string> origins = [.. match.Groups["attribute"].Captures.Select(c => c.Value)];
    Collection<string> namespaces = [..
      from o in origins
      let colon = o.IndexOf(':', SCO)
      select colon != DNE ? o[..colon] : SE];
    Collection<string> keys = [.. match.Groups["a_name"].Captures.Select(c => c.Value)];
    Collection<string> values = [.. match.Groups["a_val"].Captures.Select(c => c.Value)];
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
  private static Collection<AttributeEntity> ParseAttributes (XElement element)
  {
    Collection<AttributeEntity> result = [];
    foreach (XAttribute attr in element.Attributes())
    {
      result.Add(new AttributeEntity()
      {
        Key = attr.Name.LocalName,
        Value = attr.Value,
        Origin = attr.ToString(),
        Namespace = attr.Name.NamespaceName.IsEmpty ? null : attr.Name.NamespaceName
      });
    }
    return result;
  }

  private static IEntity? Generate (Match match, ParsingContext context)
  {
    if (!match.Success)
    {
      return new ErrorEntity() { Message = "Match was not a success: " + match.Value };
    }

    if (!context.ParsingSet!.TryGetOptions(match, context, out EntityInfo? options))
    {
      return new ErrorEntity() { Message = "No entity match: " + match.Value };
    }

    return options.Type switch
    {
      BT.Omit => null,
      BT.Operator => new SymbolEntity()
      {
        Content = options.ConstantValue ?? match.Value,
        Origin = match.Value
      },
      BT.String => new StringEntity()
      {
        Value = match.Groups["value"].Value,
        Origin = match.Value
      },
      BT.Placeholder when match.HasValidGroup("element") && match.HasValidGroup("element") => new()
      {
        Name = match.Groups["name"].Value,
        Namespace = match.HasValidGroup("ns") ? match.Groups["ns"].Value : null,
        Origin = match.Value,
      },
      BT.Number => new NumberEntity {
        Value = decimal.Parse(match.Groups["name"].Value),
        Origin = match.Value,
      },
      BT.Boolean => GetBoolean(match),
      BT.Null => GetNull(match),
      BT.Comment => new CommentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      BT.IgnoredWhitespace => new WhitespaceEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      BT.Array => new ArrayEntity()
      {
        Origin = match.Value
      },
      BT.Object => new ObjectEntity()
      {
        Origin = match.Value
      },
      BT.Raw => new RawEntity()
      {
        Origin = match.Value,
      },
      BT.Invalid => throw new InvalidOperationException("Type was Invalid."),
      BT.Absent => throw new InvalidOperationException("Type was Absent."),
      BT.Placeholder => throw new InvalidOperationException("Type was Placeholder."),
      BT.Document => new DocumentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      BT.LooseContent => new ContentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      BT.Element when match.HasValidGroup("name") && match.HasValidGroup("single") => new ElementEntity()
      {
        Origin = match.Value,
        Name = match.Groups["name"].Value,
        Attributes = ParseAttributes(match),
      },
      BT.Element when match.HasValidGroup("name") => new ElementEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
      BT.Element when match.HasValidGroup("close") => null,
      BT.Attribute when match.HasValidGroup("Key") => new AttributeEntity() { Origin = match.Value, Key = Get, Value = match.Value, },
      BT.Section when match.HasValidGroup("name") => new SectionEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
      BT.Property when match.HasValidGroup("Key") => new PropertyEntity() { Origin = match.Value, Key = key },
      BT.Operator => new SymbolEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      BT.External => throw new NotImplementedException(),
      _ => throw new InvalidOperationException($"The entity type {options.Type} is not supported."),
    };
  }
               
  private static IEntity CheckXMLMatch (Match match, ParsingContext context)
  {
    if (!match.Success) throw new InvalidOperationException("Match was not a success.");

    IEntity gen = Generate(match, context);

    return gen is ErrorEntity ee ? throw new InvalidOperationException(ee.Message) : gen;
  }
  // if (match.HasValidGroup("header")) return GetHeader (match);
  // if (match.HasValidGroup("close")) return GetClose (match);
  // if (match.HasValidGroup("single")) return GetElement (match);
  // if (match.HasValidGroup("element")) return GetOpen (match);
  // if (match.HasValidGroup("content")) return GetContent (match);
  // if (match.HasValidGroup("comment")) return GetComment (match);
  // if (match.HasValidGroup("ws")) return GetWhitespace (match);

  private static DocumentEntity CheckJSONMatch (Match match, ParsingContext context)
  {
    if (!match.Success) throw new InvalidOperationException("Match was not a success.");

    IEntity gen = Generate(match, context);

    return gen is ErrorEntity ee ? throw new InvalidOperationException(ee.Message) : (DocumentEntity) gen;
  }

  public static DocumentEntity FromXElement (XElement root, ParsingContext? context)
  {
    context ??= new() { OriginText = root.Value };
    DocumentEntity document = new()
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

    parent.AddAttributes(ParseAttributes(root));
    parent.AddChildren([.. root.Elements().Select(xe => FromXElement(xe, context))]);

    return document;
  }
  public static DocumentEntity JSONFromString (string content)
  {
    DocumentEntity top_doc = new()
    {
      Origin = content,
      Content = content,
    };
    ParsingContext context = new()
    {
      WorkingSet = [.. JSON_PreCompiled.Matches(content)],
      Document = top_doc,
      OriginText = content,
      CurrentIndex = 0,
      ParsingSet = DefaultParsingSets.JSON,
      Parent = top_doc
    };

    //string obj_pop_key ()
    //{
    //  if (keys[context._depth] is null)
    //  {
    //    throw new InvalidOperationException($"Key is not set for this object at depth {context._depth}.");
    //  }
    //  else
    //  {
    //    string result = keys[context._depth]!;
    //    keys[context._depth] = null;
    //    return result;
    //  }
    //}

    int max = context.WorkingSet.Count;

    for (int i = 0; i < max; i++)
    {
      Match match = context.CurrentItem;
      IEntity item = CheckJSONMatch(match, context);

      switch (item)
      {
        // Ignore comments
        case CommentEntity ce:
          continue;
        // Object start
        case SymbolEntity se when se == "{":
          ObjectEntity child_obj = new();
          if (parent is ObjectEntity oe)
            oe.AddProperty(child_obj);
          else if (parent is ArrayEntity ae)
            ae.AddValue(child_obj);
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
        case StringEntity se when parent is ObjectEntity oe && keys[context._depth] is null:
          obj_set_key(se.Value);
          continue;
        // The keyname is not empty, and we have a primitive entity, so this is the value for the current property.
        case StringEntity or NumberEntity or NullEntity or BooleanEntity when parent is ObjectEntity oe && obj_chk_key():
          string keyname = obj_pop_key();
          PropertyEntity prop = new()
          {
            Key = keyname,
            Origin = $"\"{keyname}\":{item.Origin}",
            Value = item,
          };
          prop.AddProperty(prop);
          continue;
        // We are in an array and we have a primitive entity
        case StringEntity or NumberEntity or NullEntity or BooleanEntity when parent is ArrayEntity ae:
          ae.AddValue(item);
          continue;
        default:
          throw new InvalidOperationException($"Unhandled Entity \"{item.Origin}\" sent to EntityFactory.");
      }
    }
    return context.Document;
  }
  public IEntity? Document { get; private set; }
  public ParsingContext Context { get; private set; }
  public void Initialize ()
  {
    
  }
  public static DocumentEntity XMLFromString (string content)
  {
    ParsingInfo info = DefaultParsingSets.XML;
    ParsingContext context = new()
    {
      ParsingSet = info,
      OriginText = content,
      WorkingSet = info.Regex!.Matches(content),
      Document = new DocumentEntity()
      {
        Origin = content,
        Content = content,
      },
    };

    while (!context.DoneWorking)
    {
      if (context.CurrentItem is Match match)
      {
        IEntity item = CheckXMLMatch(match, context);

        switch (item)
        {
          case ElementEntity ee when ee.IsHeader:
            document.SetHeader(item);
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
            document.SetRoot(parent);
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
      else if (context.CurrentItem is IEntity)
      {

      }
    }
    return document;
  }
  public static DocumentEntity FromString (string content, BT type) => type switch
  {
    BT.Element => XMLFromString(content),
    BT.Object => JSONFromString(content),
    _ => throw new InvalidOperationException($"Invalid BasicType ({type}) sent to EntityFactory."),
  };
}
