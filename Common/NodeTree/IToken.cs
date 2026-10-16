#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public interface IToken : IIndexSortable, ITextSerializer
{
  IEnumerable<string> Value { get; }
  string? Name { get; init; }
  Range Position { get; init; }
}
