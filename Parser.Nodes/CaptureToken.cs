#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;

/// <summary>A token that represents a single capture of a regular expression.</summary>
public readonly struct CaptureToken : IToken, IEquatable<IToken>, IEquatable<CaptureToken>, IComparable<IToken>, IComparable<CaptureToken>
{
  public string Content { get; init; }
  public string Type { get; init; }
  public required int CaptureIndex { get; init; }
  public required int Index { get; init; }
  public int Length => Content.Length;

  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  /// <remarks>Format: {Group}[{CaptureIndex}] = {Value}</remarks>
  public override string ToString () => $"{Type}[{CaptureIndex}] = {Content}";
  public string Serialize () => Content;

  public override bool Equals (object? obj) =>
    obj is IToken token && Equals(token);
  public override int GetHashCode () => HashCode.Combine(Type, Content, Index);
  public bool Equals (IToken? other) =>
    other is not null &&
    Type == other.Type &&
    Content == other.Content &&
    Index.Equals(other.Index);
  bool IEquatable<CaptureToken>.Equals (CaptureToken other) => Equals(other);
  int IComparable<IToken>.CompareTo (IToken? other) => CompareTo(other);
  int IComparable<CaptureToken>.CompareTo (CaptureToken other) => CompareTo(other);
  public void Print (int indent)
  {

  }

  bool IPrintable.HasHeaderLine => false;

  public static bool operator == (CaptureToken left, IToken right) => left.Equals(right);
  public static bool operator != (CaptureToken left, IToken right) => !(left == right);
  public static bool operator < (CaptureToken left, IToken right) => left.CompareTo(right) < 0;
  public static bool operator <= (CaptureToken left, IToken right) => left.CompareTo(right) <= 0;
  public static bool operator > (CaptureToken left, IToken right) => left.CompareTo(right) > 0;
  public static bool operator >= (CaptureToken left, IToken right) => left.CompareTo(right) >= 0;
}
