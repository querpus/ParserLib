#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public interface IToken : IIndexSortable, ITextSerializer
{
  /// <summary>The value of this token. This will be <see langword="null"/> if the token is composed of multiple tokens.</summary>
  string? Value { get; }
  /// <summary>The name or type of this token.</summary>
  string? Name { get; }
  Range Position { get; }
}
