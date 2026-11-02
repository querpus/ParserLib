#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;

public readonly struct CaptureToken : IToken, IEquatable<IToken>
{
  public string Value { get; init; }
  public required string? Name { get; init; }
  public required int CaptureIndex { get; init; }
  public required Range Position { get; init; }
  int IIndexSortable.Index => Position.Start.Value;
  public int Length => Position.End.Value - Position.Start.Value - 1;

  public int CompareTo (IIndexSortable? other) => Position.Start.Value.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  /// <remarks>Format: {Group}[{CaptureIndex}] = {Value}</remarks>
  public override string ToString () => $"{Name}[{CaptureIndex}] = {Value}";
  public string Serialize () => Value;

  public override bool Equals (object? obj) =>
    obj is IToken token && Equals(token);
  public override int GetHashCode () => HashCode.Combine(Name, Value, Position);
  public bool Equals (IToken? other) =>
    other is not null &&
    Name == other.Name &&
    Value == other.Value &&
    Position.Equals(other.Position);
  public static bool operator == (CaptureToken left, CaptureToken right) => left.Equals(right);
  public static bool operator != (CaptureToken left, CaptureToken right) => !(left == right);
}
