using Common.Nodes;

namespace Parser.Tokens;

/// <summary>A collection of tokens, use this to keep token operations consistent.</summary>
public sealed class TokenCollection () : IReadOnlyList<IToken>, IMultiToken<IToken>, IPrintable, IToken
{
  #region Private Fields
  /// <summary>The internal token list.</summary>
  private readonly List<IToken> _tokens = [];
  #endregion

  public TokenRef? AssignTo { get; set; }

  /// <summary>Creates the collection from a collection of tokens.</summary>
  /// <param name="tokens">The tokens to add to the list.</param>
  public TokenCollection (IEnumerable<IToken> tokens) : this() => _tokens = tokens.Any() ? [.. tokens] : [];
  public Spec Spec => _tokens[0].Spec;
  /// <summary>Gets the token at a given index.</summary>
  /// <param name="index">The index to retrieve.</param>
  /// <returns>A token at the specified index.</returns>
  public IToken this[int index] => _tokens[index];
  int IComparable.CompareTo (object? other) => CompareTo(other is IIndexSortable isort ? isort : null);

  public int Count => _tokens.Count;
  public string Type { get; set; } = SE;

  public bool HasType => !(Type is null or "-" or EmptyString || Type.Like("None"));
  public IToken? Parent { get; set; }
  public IList<IToken> Children => _tokens;
  public int Index => _tokens.Count > 0 ? _tokens[0].Index : -1;
  public string ContentNoNewLine => this.ToString();
  string IToken.Content => ToString();
  string? Common.Nodes.IToken.Value => null;
  public string Delim { get; init; } = SE;

  string? Common.Nodes.IToken.Name => Type;

  Range Common.Nodes.IToken.Position => Children.Count switch
  {
    < 1 => new(-1, -1),
    1 => Children[0].Position,
    > 1 => new(Children[0].Position.Start, Children[^1].Position.End)
  };

  public void Add (IToken item)
  {
    item.ThrowIfNull();
    _tokens.Add(item);
  }
  public void Print (int indent)
  {
    DebugIn("TokenCollection", nameof(Print));
    if (Count == 0)
    {
      LogPart(MsgClass.Error, "Empty Token List Printed. Check Why.");
    }
    if (Count == 1)
    {
      _tokens[0].Print(indent);
    }
    else
    {
      LogPart(MsgClass.GreenInfo, $"({Count} Tokens)");
      foreach (IToken item in _tokens)
      {
        NewLine();
        LogPart(MsgClass.Forced, new(' ', indent));
        item.Print(indent + 2);
      }
    }
    DebugOut();
  }
  public void Clear () => _tokens.Clear();
  public IEnumerator<IToken> GetEnumerator () => _tokens.GetEnumerator();
  public void Insert (int index, IToken item)
  {
    item.ThrowIfNull();
    _tokens.Insert(index, item);
  }

  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

  /// <summary>Removes <paramref name="count"/> from the collection starting at <paramref name="first"/>.</summary>
  /// <param name="first">The first token to remove.</param>
  /// <param name="count">The number of tokens to remove.</param>
  public void Remove (int first, int count)
  {
    _tokens.ThrowIfNull();
    for (int i = first; i < first + count; i++)
    {
      _tokens.RemoveAt(first);
    }
  }

  public void SortByIndex () => _tokens.Sort((item, item2) => item.CompareTo(item2));

  internal string GetContent () => Count == 0 ? SE : _tokens.Select(s => s.Content).Aggregate((first, second) => $"{first} {second}");

  public override string ToString () => $"TokenCollection Type {ListString(2)}";

  public string ToString (int indent) => ListString(indent);

  public string ListString (int indent)
  {
    string indent2 = new(' ', indent);
    string ret = $"{(Type.IsEmpty ? "None" : Type)} : {Count} Items";

    foreach (IToken item in _tokens)
    {
      ret += $"\n{indent2}{item}";
    }
    return ret;
  }

  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public bool Equals (IToken? other) => other is IMultiToken<IToken> tc && _tokens.SequenceEqual(tc.Children);
  public override bool Equals (object? obj) => obj is not null && obj is TokenCollection tc && Equals(tc);
  public override int GetHashCode () => _tokens.GetHashCode();
  string ISerializable.Serialize () => Children.Select(c => c.Serialize()).TextJoin(" ");

  public static bool operator == (TokenCollection left, TokenCollection right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (TokenCollection left, TokenCollection right) => !(left == right);
  public static bool operator < (TokenCollection left, TokenCollection right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (TokenCollection left, TokenCollection right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (TokenCollection left, TokenCollection right) => left?.CompareTo(right) > 0;
  public static bool operator >= (TokenCollection left, TokenCollection right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
