namespace Parser;

/// <summary>The data for a loop or cursor object.</summary>
public sealed class CursorData
{
  private XParser? Parser { get; }
  public int Length
  {
    get => field = ListKey is null ? field : Parser?.Data.GetCountOfKey(ListKey) ?? 0;
    set;
  }
  public object? Current => ListKey is null ? null :
    (Parser?.Data.TryLoadArray(ListKey, out IEnumerable<object>? list) ?? false) ? new List<object>(list)[Index] : null;
  /// <summary>The index of the cursor.</summary>
  public int Index { get; set; }
  /// <summary>The key the cursor is operating on.</summary>
  public string? ListKey { get; init; }
  public bool AtEnd => Index >= Length;

  #region Constructors
  /// <summary>Creates a new cursor.</summary>
  /// <param name="parser">The parser reference.</param>
  /// <param name="index">The index of the cursor.</param>
  /// <param name="list_key">The key to iterate through.</param>
  [SetsRequiredMembers]
  public CursorData (XParser parser, int index, string? list_key = null)
  {
    Index = index;
    ListKey = list_key;
    Parser = parser;
  }
  /// <summary>Creates a new cursor.</summary>
  /// <param name="parser">The parser reference.</param>
  /// <param name="index">The index of the cursor.</param>
  /// <param name="length">The maximum iterations.</param>
  [SetsRequiredMembers]
  public CursorData (XParser parser, int index, int length)
  {
    Index = index;
    Length = length;
    Parser = parser;
  }
  /// <summary>Creates a new cursor without a constraint.</summary>
  /// <param name="parser">The parser reference.</param>
  [SetsRequiredMembers]
  public CursorData (XParser parser)
  {
    Parser = parser;
    Index = DNE;
    Length = DNE;
  }
  #endregion

  public override string ToString () =>
    $"CursorData: {(ListKey is null ? "" : $"ListKey = {ListKey}")} ( {Index} / {Length} ) = {Current}";
}
