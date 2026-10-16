#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public readonly struct CaptureToken : IToken
{
  public string Value { get; init; }
  IEnumerable<string> IToken.Value => [Value];
  public required string? Group { get; init; }
  public required int CaptureIndex { get; init; }
  public required Range Position { get; init; }
  int IIndexSortable.Index => Position.Start.Value;
  string? IToken.Name
  {
    get => Group;
    init => Group = value;
  }

  public int CompareTo (IIndexSortable? other) => Position.Start.Value.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  /// <remarks>Format: {Group}[{CaptureIndex}] = {Value}</remarks>
  public override string ToString () => $"{Group}[{CaptureIndex}] = {Value}";
  public string Serialize () => Value;
}
public readonly struct GroupToken : IToken
{
  private readonly Collection<string> _values;
  IEnumerable<string> IToken.Value => [.. _values];
  public required string? Group { get; init; }

  public required Range Position { get; init; }
  int IIndexSortable.Index => Position.Start.Value;
  string? IToken.Name
  {
    get => Group;
    init => Group = value;
  }

  public int CompareTo (IIndexSortable? other) => Position.Start.Value.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  /// <summary>Displays a brief summary of this token's data.</summary>
  /// <returns>A string representation of this token's data.</returns>
  /// <remarks>Format: {Group}[{CaptureIndex}] = {Value}</remarks>
  public override string ToString () => $"{Group} = {Value}";
  string ITextSerializer.Serialize () => Value;
}
