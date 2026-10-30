#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Tokens;
public interface IToken : IIndexSortable, ISerializable
{
  /// <summary>The value of this token. This will be <see langword="null"/> if the token is composed of multiple tokens.</summary>
  string Value { get; }
  /// <summary>The name or type of this token.</summary>
  string Name { get; }
  /// <summary>The index of the first character of this token.</summary>
  int Start { get; }
}

public interface IMultiToken : IToken
{
  IEnumerable<IToken> Children { get; }
  int Count { get; }
  void Add (IToken token);
}

public interface ILookupToken<TKey> : IMultiToken
{
  ILookup<TKey, IToken> Lookup { get; }
  IEnumerable<TKey> Keys { get; }
  void Add (TKey key, IToken token);
}
