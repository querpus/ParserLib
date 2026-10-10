#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public readonly struct Token : IIndexSortable
{
  public required string Value { get; init; }
  public required string? Group { get; init; }
  public required int CaptureIndex { get; init; }
  public required Range Position { get; init; }
  public int Index => Position.Start.Value;
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj is IIndexSortable iSort ? iSort : null);
  public override string ToString () => $"{Group}[{CaptureIndex}] = {Value}";
}
