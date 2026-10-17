#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public readonly struct MatchToken : IToken
{
  /// <summary>The full match text.</summary>
  public string Value { get; init; }
  /// <summary>The name or type of this token.</summary>
  public required string? Name { get; init; }
  /// <summary>The match index within the <see cref="MatchCollection"/>.</summary>
  public required int MatchIndex { get; init; }
  public required Range Position { get; init; }
  int IIndexSortable.Index => Position.Start.Value;
  /// <summary>The data from the match groups.</summary>
  public Dictionary<string, dynamic> GroupData { get; set => field.AddRange(value); }
  public int CompareTo (IIndexSortable? other) => Position.Start.Value.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  public override string ToString ()
  {
    string header = $"{Name}[{MatchIndex}] = {Value.ToString(true)}" + Chars.LFs;
    string content = SE;
    foreach (var prop in GroupData)
    {
      content += $"  {prop.Key} = ";

      if (prop.Value is string s)
      {
        content += s.ToString(true) + Chars.LFs;
      }
      else if (prop.Value is IEnumerable<string> list)
      {
        content += $"[{list.Select(s => s.ToString(true)).TextJoin(", ")}]" + Chars.LFs;
      }
    }

    return header + content;
  }

  public string Serialize () => Value;
}
