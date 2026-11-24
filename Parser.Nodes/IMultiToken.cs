#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;

/// <summary>This is a token that is a list of tokens. This is useful for regex <see cref="Group"/> Tokenizers and regex <see cref="Match"/> Tokenizers.</summary>
public interface IMultiToken : IToken
{
  /// <summary>The delimeter to use when getting the <see langword="string"/> value of this token from its children.</summary>
  string Delim { get; init; }
  string IToken.Content => Content;
  /// <summary>The text content of this token.</summary>
  /// <value>This will be the combined contents of all tokens for <see cref = "IMultiToken" /> tokens.</value>
  new virtual string Content => Children.Select(s => s.Content).TextJoin(Delim);
  IList<IToken> Children { get; }
  void Add (IToken token);
}
