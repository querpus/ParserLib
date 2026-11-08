#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public class MatchToken : IToken
{
  /// <summary>The full match text.</summary>
  public string Value { get; init; } = SE;
  /// <summary>The name or type of this token.</summary>
  public string Name { get; init; } = "None";
  /// <summary>The match index within the <see cref="MatchCollection"/>.</summary>
  public int MatchIndex { get; init; }
  public int Index {get; init;}
  /// <summary>The data from the match groups.</summary>
  public Dictionary<string, OneOrMany<string>> GroupData { get; init; } = [];
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  public override bool Equals (object? obj) => obj is MatchToken mt && mt.Name.Like(Name) && mt.Value.Is(Value) && mt.Index == Index;
  public override int GetHashCode () => HashCode.Combine(Name, Value, Index);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  public override string ToString ()
  {
    string header = $"{Name}[{MatchIndex}] = {Value.ToString(true)}" + Chars.LFs;
    string content = SE;

    if (GroupData is null)
      return header;

    foreach (var prop in GroupData)
    {
      content += $"  {prop.Key} = ";

      OneOrMany<string> pval = prop.Value;

      if (pval.IsSingle)
      {
        content += pval.Value.ToString(true) + Chars.LFs;
      }
      else if (pval.IsMany)
      {
        content += $"[{pval.Values.Select(s => s.ToString(true)).TextJoin(", ")}]" + Chars.LFs;
      }
      else
      {
        content += "<no value>";
      }
    }

    return header + content;
  }
  public static implicit operator MatchToken (CaptureToken ctoken)
  {
    MatchToken mtoken = new()
    {
      Name = ctoken.Name ?? "None",
      Value = ctoken.Value,
      Index = ctoken.Index,
      MatchIndex = ctoken.CaptureIndex,
      GroupData = []
    };

    if (ctoken.Name is not null)
    {
      mtoken.GroupData.Add(ctoken.Name, ctoken.Value);
    }

    mtoken.GroupData.Add("0", ctoken.Value);

    return mtoken;
  }
  public static implicit operator MatchToken (Match match)
  {
    MatchToken mtoken = new()
    {
      Value = match.Value,
      Index = match.Index,
    };

    return mtoken;
  }
  public string Serialize () => Value;
}
