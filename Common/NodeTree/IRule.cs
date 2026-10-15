#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public interface IRule<IToken> : IEquatable<IToken>
{
  bool IsSatisfiedBy (IToken token);
}
