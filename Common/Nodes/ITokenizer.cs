#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

namespace Common.Nodes;
public interface ITokenizer<out TToken> where TToken : IToken
{
  IEnumerable<TToken> Tokens { get; }
  IEnumerable<TToken> Tokenize (string input);
  /// <summary>The rules that this tokenizer requires to process data.</summary>
  dynamic? Rules { get; init; }
  int Count => Tokens.ICount;
}
