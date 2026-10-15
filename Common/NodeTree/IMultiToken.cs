#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public interface IMultiToken : IToken
{
  string Delim { get; init; }
  string IToken.Value => Value.Count > 0 ? Value.TextJoin(Delim) : SE;
  new Collection<IToken> Value { get; }
}
