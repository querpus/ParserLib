#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public sealed class Token : IToken, IPrintable
{
  public string Delim { get; init; } = " ";
  public TokenRef? AssignTo { get; set; }
  public required Spec Spec { get; init; }
  public string Content { get; set; } = SE;
  // Calculated Properties
  public int LastPosition => Index + Length - 1;
  public int Length => Content.Length;
  public int Index { get; init; }
  public string Type { get => field.IsEmpty ? "None" : field; set; } = "None";
  public IToken? Parent { get; set; }
  public IList<IToken> Children { get; } = [];
  public bool HasType => Type is not null;

  Collection<Common.NodeTree.IToken> Common.NodeTree.IMultiToken.Value => [.. Children];
  string? Common.NodeTree.IToken.Name { get => Type; init => Type = value ?? "None"; }
  Range Common.NodeTree.IToken.Position { get => new(Index, LastPosition); init => Index = value.Start.Value; }

  int IComparable.CompareTo (object? other) => CompareTo(other is IIndexSortable isort ? isort : null);
  public override string ToString () => $"{Type} : {Content}";
  public string ToString (int indent) => $"{Type} : {Content}";
  public void Print (int indent)
  {
    LogPart(MsgClass.Forced, $"{Type}");
    LogPart(MsgClass.BlueInfo, " : ");
    LogPart(MsgClass.GreenInfo, ((IToken) this).ContentNoNewLine);
  }
  public override bool Equals (object? obj) => obj is IToken rt && Equals(rt);
  public override int GetHashCode () => HashCode.Combine(Content, Index, Type);
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public bool Equals (IToken? other) => GetHashCode() == other?.GetHashCode();
  public string Serialize () => Content;
  public static bool operator == (Token left, IToken right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (Token left, IToken right) => !(left == right);
  public static bool operator < (Token left, IToken right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (Token left, IToken right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (Token left, IToken right) => left?.CompareTo(right) > 0;
  public static bool operator >= (Token left, IToken right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
