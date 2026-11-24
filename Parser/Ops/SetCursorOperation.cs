namespace Parser.Ops;

public sealed class SetCursorOperation () : Operation
{
  public required string CursorKey { get; init; }
  public int Position { get; init; } = -1;
  public string? PositionKey { get; init; }

  protected override void Execute ()
  {
    CursorData cursor = Data.GetCursorByKey(CursorKey);
    cursor.Index = Position == -1 ? Data[PositionKey] is int i ? i : 0 : Position;
  }
}

public sealed class SpecProcessOperation () : Operation
{
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }

  protected override void Execute ()
  {
    TokenCollection tokens = Data[InputKey] as TokenCollection ?? [];
    Spec? newSpec = null;
    int eof = tokens.Count;

    for (int pos = 0; pos < eof; pos++)
    {

    }

    Data[OutputKey] = newSpec!;
  }
}
