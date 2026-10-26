#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

namespace Common.Nodes;
public interface ITokenizer<TToken>
{
  Collection<TToken> Tokens { get; }
  Collection<TToken> Tokenize (string input);
  TokenRuleSet? Rules { get; init; }
  int Count => Tokens.Count;
}
