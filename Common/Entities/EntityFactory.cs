#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;
using System.Xml.Linq;

namespace Common.Entities;

public static class EntityFactory
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
    return [.. result];
  }

  private static IEntity? Generate (Match match, ParsingContext context)
  {
    if (!match.Success)
    {
      return new ErrorEntity() { Message = "Match was not a success: " + match.Value };
    }

    if (!context.ParsingSet!.TryGetOptions(match, out EntityInfo? options))
    {
      return new ErrorEntity() { Message = "No entity match: " + match.Value };
    }

    IEntity? entity = null;

    if (options.Class is null)
      goto Logic;

    entity = options.Class.InvokeMember(SE, BFCI, null, null, [], CIIC) as IEntity;
    entity = entity switch
    {
      null => null,
      SymbolEntity => new SymbolEntity
      {
        Content = options.ConstantValue ?? match.Value,
        Origin = match.Value
      },
      StringEntity => new StringEntity
      {
        Value = match.Groups["value"].Value,
        Origin = match.Value
      },
      NumberEntity => new NumberEntity
      {
        Value = decimal.Parse(match.Groups["name"].Value, CIIC),
        Origin = match.Value,
      },
      BooleanEntity => new BooleanEntity
      {
        Value = bool.Parse(match.Groups["value"].Value),
        Origin = match.Value,
      },
      NullEntity => new NullEntity
      {
        Origin = match.Value,
      },
      CommentEntity => new CommentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      WhitespaceEntity => new WhitespaceEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      ArrayEntity => new ArrayEntity()
      {
        Origin = match.Value
      },
      ObjectEntity => new ObjectEntity()
      {
        Origin = match.Value
      },
      RawEntity => new RawEntity()
      {
        Origin = match.Value,
      },
      ErrorEntity => throw new InvalidOperationException("Type was Invalid."),
      DocumentEntity => new DocumentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      ContentEntity => new ContentEntity()
      {
        Content = match.Value,
        Origin = match.Value
      },
      ElementEntity when match.HasValidGroup("name") && match.HasValidGroup("single") => new ElementEntity()
      {
        Origin = match.Value,
        Name = match.Groups["name"].Value,
        Attributes = ParseAttributes(match),
      },
      ElementEntity when match.HasValidGroup("name") => new ElementEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
      ElementEntity when match.HasValidGroup("close") => null,
      //BT.Attribute when match.HasValidGroup("Key") => new AttributeEntity() { Origin = match.Value, Key = match.Groups["key"].Value },
      SectionEntity when match.HasValidGroup("name") => new SectionEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
      PropertyEntity when match.HasValidGroup("Key") => new PropertyEntity() { Origin = match.Value, Key = match.Groups["key"].Value },
      AttributeEntity => throw new InvalidOperationException("Attributes are handled in ParseAttributes."),
      IEntity => options.Class.InvokeMember(SE, BFCI, null, null, [], CIIC) as IEntity,
    };

  Logic:

    if (options.SetPropKey)
    {
      context.SetDepthProperty("Property", entity);
    }
    if (options.SetAsNextLevelParent)
    {
      context.SetDepthProperty("NextParent", entity);
    }
    if (options.AddToPropKey)
    {
      context.GetDepthProperty<PropertyEntity>("Property")?.Value = entity;
    }

    if (entity is not null)
    {
      if (context.Parent is not null)
        entity.SetParent(context.Parent);
      context.Parent?.AddChild(entity);
    }

    if (options.DepthChange > 0)
    {
      IEntity? new_parent = context.HasDepthProperty("NextParent")
        ? context.GetDepthProperty<IEntity>("NextParent")
        : (entity ?? options.ChildType?.InvokeMember(SE, BFCI, null, null, null, CIIC)) as IEntity;
      context.Descend(options.DepthChange, [], new_parent!);
      context.Parent = new_parent;
    }
    if (options.DepthChange < 0)
    {
      context.Ascend(options.DepthChange);
    }
    return entity;
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
  public static DocumentEntity FromString (string content, ParsingInfo info)
  {
    if (info.SingleObject is null)
    {

    }

    ParsingContext context = new()
    {
      ParsingSet = info,
      OriginText = content,
      Document = new DocumentEntity()
      {
        Origin = content,
        Content = content,
      },
    };
    MatchCollection matches = info.Regex!.Matches(content);
    List<IEntity> entities = [];
    foreach (Match match in matches.Cast<Match>())
    {
      IEntity? entity = Generate(match, context);
      if (entity is not null) entities.Add(entity);
    }
    return context.Document;
  }
}
