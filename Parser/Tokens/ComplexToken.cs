#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public sealed class ComplexToken : IToken, IPrintable
{
  private readonly Dictionary<TokenRef, IToken> _token_pieces = [];
  public TokenRef? AssignTo { get; set; }
  public Dictionary<TokenRef, IToken?> TokenPieces
  {
    init => _token_pieces = [.. value];
    get => [.. _token_pieces];
  }
  public Dictionary<string, IToken> CustomProperties { get; } = [];
  public IToken this[TokenRef piece_type]
  {
    get => _token_pieces[piece_type];
    set => _token_pieces[piece_type] = value;
  }

  public IToken? Name => GetPiece(TokenRef.Name);
  public IToken? ObjType => GetPiece(TokenRef.Type);
  /// <summary>Value is simply the first item in the ValueList.</summary>
  public IToken? Value => (GetPiece(TokenRef.ValueList) as TokenCollection)?[0];
  public IToken? Left => GetPiece(TokenRef.Left);
  public IToken? Right => GetPiece(TokenRef.Right);
  public IToken? Center => GetPiece(TokenRef.Center);
  public TokenCollection? AddFlags => (TokenCollection?) GetPiece(TokenRef.AddFlagList);
  public TokenCollection? SubFlags => (TokenCollection?) GetPiece(TokenRef.SubFlagList);
  public TokenCollection? Properties => (TokenCollection?) GetPiece(TokenRef.PropertyList);
  public TokenCollection? Parameters => (TokenCollection?) GetPiece(TokenRef.ParameterList);
  public TokenCollection? Statements => (TokenCollection?) GetPiece(TokenRef.StatementList);
  public TokenCollection? Values => (TokenCollection?) GetPiece(TokenRef.ValueList);

  public string Content => Children.Select(i => i.Content).TextJoin(" ");
  public IReadOnlyCollection<TokenRef> PiecesPresent => [.. _token_pieces.Keys.Where(kvp => kvp.IsUsed(_token_pieces))];
  public string Type { get; set; } = SE;
  public bool HasType => Type.IsNotEmpty && !Type.Like("None") && !Type.Like("-");
  public IToken? Parent { get; set; }
  public IList<IToken> Children { get; init; } = [];
  public int Index => Children.Count > 0 ? Children[0].Index : -1;
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public bool Equals (IToken? other) => other is ComplexToken && Children.SequenceEqual(other.Children);
  public IToken? GetPiece (TokenRef piece_type) => _token_pieces.TryGetValue(GetListID(piece_type), out IToken? value) ? value : null;
  public string? GetPieceContent (TokenRef piece_type) => _token_pieces.TryGetValue(GetListID(piece_type), out IToken? value) ? value.Content : null;
  public bool HasPieceType (TokenRef piece_type) => piece_type.IsUsed(_token_pieces);
  public bool HasPieceTypes (TokenRef[] piece_types) => piece_types.All(p => p.IsUsed(_token_pieces));
  private static TokenRef GetListID (TokenRef itemID) => itemID switch
  {
    TokenRef.Value => TokenRef.ValueList,
    TokenRef.Property => TokenRef.PropertyList,
    TokenRef.Statement => TokenRef.StatementList,
    TokenRef.Parameter => TokenRef.ParameterList,
    TokenRef.AddFlag => TokenRef.AddFlagList,
    TokenRef.SubFlag => TokenRef.SubFlagList,
    _ => itemID
  };
  private static bool HasListID (TokenRef itemID) => itemID != GetListID(itemID);
  public void AddPieceType (TokenRef piece_type, IToken token)
  {
    if (!HasListID(piece_type))
    {
      SetPieceType(piece_type, token);
      return;
    }
    if (!HasPieceType(piece_type))
    {
      TokenCollection new_list = [token];
      SetPieceType(GetListID(piece_type), new_list);
      SetPieceType(piece_type, new_list);
      return;
    }

    IToken piece = GetPiece(piece_type)!;

    if (piece is TokenCollection tc)
    {
      if (tc.Count == 0)
      {
        TokenCollection new_list = [token];
        SetPieceType(GetListID(piece_type), new_list);
        SetPieceType(piece_type, new_list);
      }
      else
      {
        tc.Add(token);
      }
    }
    else
    {
      TokenCollection new_list = [piece, token];
      SetPieceType(GetListID(piece_type), new_list);
      SetPieceType(piece_type, new_list);
    }
  }
  public void AddPiecesToType (TokenRef piece_type, TokenCollection tokens)
  {
    if (tokens is null || tokens.Count == 0)
      return;

    foreach (IToken token in tokens)
    {
      AddPieceType(piece_type, token);
    }
  }
  private void SetPieceType (TokenRef piece_type, IToken token) => _token_pieces[piece_type] = token;
  public override bool Equals (object? obj) => obj is IToken ct && Equals(ct);
  public override int GetHashCode () => _token_pieces.GetHashCode();
  public static bool operator == (ComplexToken left, ComplexToken right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (ComplexToken left, ComplexToken right) => !(left == right);
  public static bool operator < (ComplexToken left, ComplexToken right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (ComplexToken left, ComplexToken right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (ComplexToken left, ComplexToken right) => left?.CompareTo(right) > 0;
  public static bool operator >= (ComplexToken left, ComplexToken right) => left is null ? right is null : left.CompareTo(right) >= 0;

  private Dictionary<string, IToken?> Parts => new()
  {
    ["Name"] = Name,
    ["Type"] = ObjType,
    ["Value"] = Value,
    ["Left"] = Left,
    ["Center"] = Center,
    ["Right"] = Right,
    ["AddFlagList"] = AddFlags,
    ["SubFlagList"] = SubFlags,
    ["ParameterList"] = Parameters,
    ["PropertyList"] = Properties,
    ["ValueList"] = Values,
    ["StatementList"] = Statements,
  };
  int IComparable.CompareTo (object? other) => CompareTo(other is IIndexSortable isort ? isort : null);
  string IToken.ContentNoNewLine => Children.Select(child => child.ToString()).TextJoin().Replace(["\n", "\r"], ["<LF>", "<CR>"]);

  public required Spec Spec { get; init; }
  /// <summary>Calls the action on every piece in this token.</summary>
  /// <param name="action">The action to call.</param>
  /// <remarks>This will not call both Value and ValueList. It chooses based on the quantity of the list.</remarks>
  private void EachPart (Action<KeyValuePair<string, IToken?>> action)
  {
    foreach (KeyValuePair<string, IToken?> part in Parts.Where(k => k.Value is not null))
    {
      if (part.Key.Like("Value") && (part.Value is not TokenCollection tc || tc.Count == 1))
      {
        action(part);
      }
      else if (part.Key.Like("ValueList") && (part.Value is TokenCollection tc2 && tc2.Count > 1))
      {
        action(part);
      }
      else if (part.Key.Like(["StatementList", "PropertyList", "AddFlagList", "SubFlagList", "ParameterList", "Left", "Right", "Center", "Type", "Name"]))
      {
        action(part);
      }
    }
  }

  public override string ToString () => ToString(0);
  public void Print (int indent)
  {
    string ind_str = new(' ', indent);
    LogPart(MsgClass.Forced, Type);
    EachPart(kvp =>
    {
      if (kvp.Value is IToken tok)
      {
        NewLine();
        LogPart(MsgClass.Hidden, ind_str);
        LogPart(MsgClass.Warning, kvp.Key);
        LogPart(MsgClass.BlueInfo, " : ");
        tok.Print(indent + 2);
      }
    });
  }
  public string ToString (int indent)
  {
    DebugIn("ComplexToken", "ToString");
    static string sp (int i) => new(' ', i);
    int spCount = indent + 2;
    string indent2 = $"{sp(spCount)}";

    string temp = $"{Type}";

    bool multiple_values = Values?.Count > 1;

    EachPart(kvp =>
    {
      if (kvp.Value is null ||
      (kvp.Value is TokenCollection tc && tc.IsEmpty) ||
      (kvp.Key is "Value" && multiple_values) ||
      (kvp.Key is "ValueList" && !multiple_values))
      {
        return;
      }

      temp += $"\n{indent2}{kvp.Key} = {kvp.Value.ToString(spCount + 2)}";
    });
    DebugOut();
    return temp;
  }

  public object Clone () => new ComplexToken()
  {
    Spec = Spec,
    TokenPieces = [.. _token_pieces],
    Children = [.. Children],
    Type = Type,
  };
}
