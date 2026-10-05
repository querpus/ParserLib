#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;
using System.Xml.Linq;

namespace Common.Entities;

public static class EntityFactory
{
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

    Stack<dynamic> parentStack = context.GetStack("Parent");
    Stack<dynamic> propertyStack = context.GetStack("Property");

    // Push 'Property' Stack
    if (options.SetPropKey && entity is not null)
    {
      propertyStack.Push(entity);
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
      if (object.ReferenceEquals (propertyStack.Peek().Parent, context.Parent))
      {
        IEntity prop = (IEntity) propertyStack.Pop();

        foreach (KeyValuePair<string, string> kvp in options.StorePieceTypes)
        {
          string prop_name = kvp.Key;
          string match_group = kvp.Value;

          string[]? caps = match.GetCaptures(match_group)! ?? throw new InvalidOperationException($"Group name {match_group} not present, cannot assign property.");

          if (caps.Length == 1)
          {
            prop.AddToDataCollection(prop_name, caps[0]);
          }
          else
          {
            prop.AddToDataCollection(prop_name, caps);
          }
        }
      }
      else
      {
        //TODO: Handle Arrays
      }
    }

    if (entity is not null)
    {
      context.Parent?.AddChild(entity);
    }

    switch (options.DepthChange)
    {
      case DepthOperation.Descend:
        context.Depth++;
        IEntity? child = (context.GetStatic("NextParent") ?? entity) ?? throw new InvalidOperationException("Child was null and could not become parent.");
        parentStack.Push(child);
        break;
      case DepthOperation.Ascend:
        context.Depth--;
        parentStack.Pop();
        break;
      case DepthOperation.AscendAndDescend:
        if (options.OnlyAscendIfParentClass is null || context.GetStack("Parent").Peek().TypeName == options.OnlyAscendIfParentClass.Name)
        {
          context.Depth--;
          parentStack.Pop();
        }
        IEntity? child2 = (context.GetStatic("NextParent") ?? entity) ?? throw new InvalidOperationException("Child was null and could not become parent.");
        parentStack.Push(child2);
        context.Depth++;
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
      Namespace = root.Name.NamespaceName.IsEmpty ? null : root.Name.NamespaceName,
    };
    context.Parent = parent;
    document.SetRoot(parent);

    parent.Add(ParseAttributes(root));
    parent.AddChildren([.. root.Elements().Select(xe => FromXElement(xe, context))]);

    return document;
  }
  public static DocumentEntity FromString (string content, ParsingInfo info)
  {
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

    MatchCollection? matches = info.Regex?.Matches(content);

    if (matches is null)
    {
      return context.Document;
    }

    Collection<IEntity> ents = [];
    foreach (Match match in matches)
    {
      IEntity? gen = Generate(match, context);

      if (gen is not null)
      {
        ents.Add(gen);

        Debug.Log(MsgClass.BlueInfo, gen.ToString()!, "EntityFactory");
      }
    }

    return context.Document;
  }
}
