#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
public class CommentStyle
{
  /// <summary>The character or character sequence that begins this comment.</summary>
  public required string Open { get; set; }
  /// <summary>The character or character sequence that ends this comment.</summary>
  /// <remarks>Leave <see langword="null"/> for linefeed or EOF terminated.</remarks>
  public string? Close { get; set; }
  /// <summary>The regular expression that captures this comment.</summary>
  [SS("Regex")]
  public required string Expression { get; set; }
}
public class QuoteStyle
{
  /// <summary>The character or character sequence that begins this <see langword="string"/> or <see langword="char"/>.</summary>
  public required string Open { get; set; }
  /// <summary>The character or character sequence that ends this <see langword="string"/> or <see langword="char"/>.</summary>
  /// <remarks>Leave this as <see langword="null"/> if it is the same as open.</remarks>
  public string? Close { get; set; }
  /// <summary>The character or character sequence that prevents this <see langword="string"/> or <see langword="char"/> from ending.</summary>
  /// <remarks>Leave this as <see langword="null"/> if there is no escape character.</remarks>
  public string? Escape { get; set; }
  /// <summary>The regular expression that captures this <see langword="string"/> or <see langword="char"/>.</summary>
  [SS("Regex")]
  public required string Expression { get; set; }
}

public class ParsingSet
{
  public ParsingInfo Global { get; init; } = new()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    TotalPasses = 2
  };
  public Collection<QuoteStyle> Comments { get; init; } = [];
  public Collection<QuoteStyle> Quotes { get; init; } = [];
  public IImmutableList<EntityInfo> EntityOptions { get; init; } = [];

  public bool TryGetOptions (Match match, ParsingContext context, [NotNullWhen(true)] out EntityInfo? options)
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
}
