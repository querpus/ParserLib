namespace Parser.Ops;

public class MakeCursorOperation : Operation
{
  public required string CursorKey { get; init; }
  public required string ContentKey { get; init; }
  public int Position { get; init; }
  protected override void Execute () => Data[CursorKey] = new CursorData(Parser)
  {
    Index = Position,
    ListKey = ContentKey
  };
}
