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

    if (options.Class is not null)
    {
      entity = options.Class.InvokeMember(SE, BFCI, null, null, [], CIIC) as IEntity;
      entity?.DoAssign(match);
    }

    // Push 'Property' Stack
    if (options.SetPropKey && entity is not null)
    {
      context.GetStack("Property")?.Push(entity);
    }
    // Set 'NextParent' Entity
    if (options.SetAsNextLevelParent && entity is not null)
    {
      context.SetStatic("NextParent", entity);
    }
    // Pops 'Property' Stack and Assigns the Value
    if (options.AddToProperty && entity is not null)
    {
      // Check if we can even add the property
      // A keyed entity list would have 1 property stored minimum from the key definition, a non-keyed collection would have nothing
      if (context.Parent?.Properties.Count > 0 && context.GetStack("Property").Count > 0)
      {
        IEntity property = context.GetStack("Property").Pop();

        foreach (KeyValuePair<string, string> kvp in options.StorePieceTypes)
        {
          string prop_name = kvp.Key;
          string match_group = kvp.Value;

          string[]? caps = match.GetCaptures(match_group)! ?? throw new InvalidOperationException($"Group name {match_group} not present, cannot assign property.");

          if (caps.Length == 1)
          {
            property.AddToDataCollection(prop_name, caps[0]);
          }
          else
          {
            property.AddToDataCollection(prop_name, caps);
          }
        }
        ((ObjectEntity) context.Parent).AddProperty((property as PropertyEntity)!);
      }
      else
      {
        //TODO: Handle Arrays
      }
    }

    if (entity is not null)
    {
      if (context.Parent is not null)
        entity.SetParent(context.Parent);
      context.Parent?.AddChild(entity);
    }

    void descend()
    {
      IEntity? child = context.GetStatic("NextParent");

      child ??= options.Class?.InvokeMember(SE, BFCI, null, null, null, CIIC) as IEntity;

      if (child is null)
        throw new InvalidOperationException("Child was null and could not become parent.");

      context.Descend(child);
    }

    switch (options.DepthChange)
    {
      case DepthOperation.Descend:
        descend();
        break;
      case DepthOperation.Ascend:
        context.Ascend();
        break;
      case DepthOperation.AscendAndDescend:
        if (options.OnlyAscendIfParentClass is null || context.GetStack("Parent").Peek().TypeName == options.OnlyAscendIfParentClass.Name)
        {
          context.Ascend();
        }
        descend();
        break;
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
    foreach (Match match in matches.Cast<Match>().Where(m => m.Success))
    {
      IEntity? entity = Generate(match, context);
      if (entity is not null) entities.Add(entity);
    }
    return context.Document;
  }
}
