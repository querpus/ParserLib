namespace Parser.Ops;

public sealed class ForCountOperation : LoopOperation
{
  public int Length { get; set; } = -1;
  public string? LengthKey { get; init; }

  protected override IOperation ContinueOp => new OperationContinue(LoopIndex, 1, CursorKey);
  protected override IOperation StartLoop => new OperationCheckCount(CursorKey, CurrentIndex);

  protected override void Execute ()
  {
    if (Length == -1)
    {
      Length = (int) Data[LengthKey];
    }

    Data[CursorKey] = new CursorData(Parser, 0, Length);
    Parser.SetNextOperationIndex(LoopIndex);
  }
}
