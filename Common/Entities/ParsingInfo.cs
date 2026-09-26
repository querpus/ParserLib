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
  [SS("regex")]
  public string? RegexString { get; init; }
  public RegexOptions RegexOptions { get; init; }
  [MaybeNull]
  public Regex Regex
  {
    get
    {
      if (field is null && RegexString is not null)
      {
        field = new Regex(RegexString, RegexOptions, new(9000));
        return field;
      }
      else
      {
        return field;
      }
    }
  }
  public IImmutableList<EntityInfo> EntityOptions { get; init; } = [];
  /// <summary>Tries to get the <see cref="EntityInfo"/> for the given <see cref="Match"/>.</summary>
  /// <param name="match">The regex match.</param>
  /// <param name="context">The parser context of the previous matches.</param>
  /// <param name="options">The output of the <see cref="EntityInfo"/> if the info is located.</param>
  /// <returns><see langword="true"/> if the match is able to select an <see cref="EntityInfo"/> that meets the requirements, <see langword="false"/> otherwise.</returns>
  public bool TryGetOptions (Match match, [NotNullWhen(true)] out EntityInfo? options)
  {
    foreach (EntityInfo info in EntityOptions)
    {
      if (info.Matches(match))
      {
        options = info;
        return true;
      }
    }
    options = null;
    return false;
  }
}
