#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public interface IMultiToken<TToken> : IToken where TToken : IToken
{
  string Delim { get; init; }
  string? IToken.Value => Value;
  new string? Value => Children.Select(s => s.Serialize()).TextJoin(Delim);
  IList<TToken> Children { get; }
}
