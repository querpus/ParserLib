#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entity.BasicType;

namespace Common.Entity;
public class CommentStyle
{
  /// <summary>The character or character sequence that begins this comment.</summary>
  public string Open { get; set; }
  /// <summary>The character or character sequence that ends this comment.</summary>
  /// <remarks>Leave <see langword="null"/> for linefeed or EOF terminated.</remarks>
  public string? Close { get; set; }
  /// <summary>The regular expression that captures this comment.</summary>
  [SS("Regex")]
  public string Expression { get; set; }
}
public class QuoteStyle
{
  /// <summary>The character or character sequence that begins this <see langword="string"/> or <see langword="char"/>.</summary>
  public string Open { get; set; }
  /// <summary>The character or character sequence that ends this <see langword="string"/> or <see langword="char"/>.</summary>
  /// <remarks>Leave this as <see langword="null"/> if it is the same as open.</remarks>
  public string? Close { get; set; }
  /// <summary>The character or character sequence that prevents this <see langword="string"/> or <see langword="char"/> from ending.</summary>
  /// <remarks>Leave this as <see langword="null"/> if there is no escape character.</remarks>
  public string? Escape { get; set; }
  /// <summary>The regular expression that captures this <see langword="string"/> or <see langword="char"/>.</summary>
  [SS("Regex")]
  public string Expression { get; set; }
}  

public class ParsingSet
{
  public ParsingInfo Global { get; init; } = new()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    TotalPasses = 2
  };
  public IImmutableList<EntityInfo> EntityOptions { get; init; } = [];

  public bool TryGetOptions(Match match, ParsingContext context, [NotNullWhen(true)] out EntityInfo? options)
  {
    options = EntityOptions.FirstOrDefault(item => {
      string[] groups = [.. match.Groups.OfType<Group>().Select(g => g.Value)];
      IndicationRule rule = item!.IndicatedItem;
      StringComparison sc = rule.IgnoreCase ? SCOIC : SCO;
      IEqualityComparer<string>? iec = rule.IgnoreCase ? CaseInsensitiveEqualityComparer.Ordinal : null;
      bool use_exact = rule.ExactValue is not null;
      bool use_group = rule.Group.IsNotEmpty;
      bool exact_pass = use_exact && rule.ExactValue!.Equals(match.Value, sc);
      bool group_pass = use_group && groups.Contains(rule.Group!, iec);
      if (!use_group && !use_exact)
      {
        string msg = !item.CreateEmptyAtStart
        ? $"Unused rule [{EntityOptions.IndexOf(item)}]"
        : $"Skipping Initial Container rule [{EntityOptions.IndexOf(item)}]";
        Debug.Log(MsgClass.Warning, msg, this);
        return false;
      }
      return (!use_group || group_pass) && (!use_exact || exact_pass);
    }, null);
    return options is not null;
  }

  private IParsedEntity IteratePieces<T> (Match match, EntityInfo options, ParsingContext context) where T : ParsedEntity, new()
  {
    T entity = new()
    {
      Origin = match.Value
    };

    if (context.Parent is not null)
      entity.SetParent(context.Parent);

    foreach (KeyValuePair<string, string> piece in options.StorePieceTypes)
    {
      entity.DataValues[piece.Key] = match.Groups[piece.Value].Captures.Count > 1
        ? match.Groups[piece.Value].Captures.Select(c => c.Value).ToCollection()
        : match.Groups[piece.Value].Value;
    }

    if (options.DepthChange != 0)
    {
      context.Depth += options.DepthChange;
    }

    if (options.AddToPropKey && context.PropKey is IEntity prop_entity)
    {
      prop_entity.DataValues.Concat(entity.DataValues);
    }

    if (options.SetPropKey)
    {
      context.PropKey = entity;
    }

    return entity;
  }

  public IEntity? Generate (Match match, EntityInfo options)
  {
    try
    {
      return options.Type switch
      {
        BT.Operator when options.ConstantValue is not null => new SymbolEntity()
        {
          Content = options.ConstantValue,
          Origin = match.Value
        },
        BT.Operator => new SymbolEntity()
        {
          Content = match.Value,
          Origin = match.Value
        },
        BT.String => new StringEntity()
        {
          Value = match.Groups[options.StorePieceTypes["Value"]].Value,
          Origin = match.Value
        },
        BT.Number => new NumberEntity()
        {
          Value = decimal.TryParse(match.Groups[options.StorePieceTypes["Value"]].Value, out decimal result)
          ? result
          : throw new InvalidValueException(match.Groups[options.StorePieceTypes["Value"]].Value),
          Origin = match.Value,
        },
        BT.Boolean => new BooleanEntity()
        {
          Value = bool.TryParse(match.Groups[options.StorePieceTypes["Value"]].Value, out bool result)
          ? result
          : throw new InvalidValueException(match.Groups[options.StorePieceTypes["Value"]].Value),
          Origin = match.Value,
        },
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
        BT.Document => new DocumentEntity() { Content = match.Value, Origin = match.Value },
        BT.LooseContent => GetContent(match),
        BT.Element when match.HasValidGroup("name") => new ElementEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
        BT.Attribute when context.Key is string key => new AttributeEntity() { Origin = match.Value, Key = key, Value = match.Value, },
        BT.Section when match.HasValidGroup("name") => new SectionEntity() { Origin = match.Value, Name = match.Groups["name"].Value },
        BT.Property when context.Key is string key => new PropertyEntity() { Origin = match.Value, Key = key, Value = GetString(match) },
        BT.Operator => GetSymbol(match),
        _ => throw new InvalidOperationException($"The entity type {options.Type} is not supported."),
      };
    }
    catch (InvalidValueException ive)
    {
      Debug.Log(MsgClass.Warning, ive.Message, this);
      return null;
    }
  }
}
