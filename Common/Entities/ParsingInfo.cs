#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class ParsingInfo
{
  /// <summary>The kind of object the end result of the operations should be.</summary>
  /// <remarks> This is for validation purposes.</remarks><value><br/>
  /// * <see langword="true"/>,The parser should make a single <see cref="IEntity"/> object from the data.<br/>
  /// * <see langword="false"/>, the parser should make a <see cref="Collection{T}"/> of <see cref="IEntity"/> objects.</value>
  public bool GeneratesSingleObject { get; init; }
  public EntityInfo? SingleObject { get; init; }
  /// <summary>The number of iterative loops the parser must go through.</summary>
  public int TotalPasses { get; init; }
  /// <summary>Whether to ignore case on non-regex matches.</summary>
  public bool IgnoreCase { get; init; }

  public string Regex { get; init; }
  public Collection<CommentStyle> Comments { get; init; } = [];
  public Collection<QuoteStyle> Quotes { get; init; } = [];
  public IImmutableList<EntityInfo> EntityOptions { get; init; } = [];
  /// <summary>Tries to get the <see cref=EntityInfo"/> for the given <see cref="Match"/>.</summary>
  /// <param name="match">The regex match.</param>
  /// <param name="context">The parser context of the previous matches.</param>
  /// <param name="options">The output of the <see cref=EntityInfo"/> if the info is located.</param>
  /// <returns><see langword="true"/> if the match is able to select an <see cref="EntityInfo"/> that meets the requirements, <see langword="false"/> otherwise.</returns>
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
