#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public interface IRule<TToken> : IEquatable<TToken>
{
  bool IsSatisfiedBy (TToken token);
}
