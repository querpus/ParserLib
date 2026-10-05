#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An interface for storing entities.</summary>
public interface IEntity : ITextSerializer, IEquatable<IEntity>
{
  IList<IEntity> Children { get; }
  Dictionary<string, object?> DataValues { get; }
  /// <summary>Returns <see langword="true"/> if the entity has been initialized successfully, <see langword="false"/> otherwise.</summary>
  bool IsValid { get; }
  /// <summary>Gets the origin of the parsed entity.</summary>
  /// <remarks>This is null for entities that are not derived from a single source.</remarks>
  string? Origin { get; set; }
  /// <summary>Static equality method.</summary>
  /// <param name="obj_a">Entity 'A'.</param>
  /// <param name="obj_b">Entity 'B'.</param>
  /// <returns><see langword="true"/> if 'A' is equal to 'B', <see langword="false"/> otherwise.</returns>
  static bool Equals (IEntity? obj_a, IEntity? obj_b) =>
    (obj_a is null && obj_b is null) || (obj_a is not null && obj_b is not null && obj_a.Equals(obj_b));
  bool Equals (object? obj);
  int GetHashCode ();
  /// <summary>Adds a child to the children list.</summary>
  /// <param name="child">The child to add.</param>
  void AddChild (IEntity child);
  void AddChildren (IEnumerable<IEntity> children);
  void AddToDataCollection (string key, object data);
  void DoAssign (Match match);
  void Assign (Match match);
  IEntity ToEntity ();
}

public readonly struct EToken : IIndexSortable
{
  public string? Value { get; init; }
  public string? Group { get; init; }
  public int CaptureIndex { get; init; }
  public Range Position { get; init; }
  public int Index => Position.Start.Value;
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  public override string ToString () => $"{Group}[{CaptureIndex}] = {Value}";
}

public class ETokenFactory
{
  [SS("regex")]
  public string Test =
    """
    (?<s_open> \[ ) (?<s_name> [^\]\n]+) (?<s_close> \] ) |
    (?<p_qt> ['"] ) (?<p_key> ((?!\k<p_qt>).)* ) \k<p_qt> \s* (?<p_eq> = ) \s* (?<p_vqt> ['"] ) (?<p_value> .* ) \k<p_vqt> |
    (?<p_key> [^\s=]+) \s* (?<p_eq> = ) \s* (?<p_vqt> ['"] ) (?<p_value> .* ) \k<p_vqt> |
    (?<p_key> [^\s=]+) \s* (?<p_eq> = ) \s* (?<p_value> [^\n]+)
    """;

  public string Input =
    """
    [Section]
    @ = property_value
    "key" = "property value"
    """;

  public Collection<EToken> CreateTokens (string input)
  {
    Collection<EToken> tokens = [];
    Regex regex = new(Test, RegexOptions.IgnorePatternWhitespace);
    MatchCollection matches = regex.Matches(input);
    foreach (Match match in matches)
    {
      foreach (KeyValuePair<string, Group> kvp in match.Groups)
      {
        Group group = kvp.Value;
        if (group.Success && kvp.Key != "0")
        {
          for (int i = 0; i < group.Captures.Count; i++)
          {
            Capture capture = group.Captures[i];
            EToken token = new()
            {
              Value = capture.Value,
              Group = kvp.Key,
              CaptureIndex = i,
              Position = capture.Index..(capture.Index + capture.Length)
            };
            tokens.Add(token);
          }
        }
      }
    }
    return tokens;
  }
}
