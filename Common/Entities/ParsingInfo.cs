#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class ParsingInfo
{
  /// <summary>The kind of object the end result of the operations should be.</summary>
  /// <remarks> This is for validation purposes.</remarks><value><br/>
  /// * <see langword="true"/>,The parser should make a single <see cref="IEntity"/> object from the data.<br/>
  /// * <see langword="false"/>, the parser should make a <see cref="Collection{T}"/> of <see cref="IEntity"/> objects.</value>
  public bool GeneratesSingleObject { get; set; }
  /// <summary>The number of iterative loops the parser must go through.</summary>
  public int TotalPasses { get; init; }
  /// <summary>Whether to ignore case on non-regex matches.</summary>
  public bool IgnoreCase { get; init; }
  public bool Equals (ParsingInfo other) =>
    GeneratesSingleObject == other.GeneratesSingleObject &&
    TotalPasses == other.TotalPasses &&
    IgnoreCase == other.IgnoreCase;
  public Collection<CommentStyle> Comments { get; init; } = [];
  public Collection<QuoteStyle> Quotes { get; init; } = [];
  public IImmutableList<EntityInfo> EntityOptions { get; init; } = [];
  /// <summary></summary>
  /// <param name="match"></param>
  /// <param name="context"></param>
  /// <param name="options"></param>
  /// <returns></returns>
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
