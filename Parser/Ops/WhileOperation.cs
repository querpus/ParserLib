namespace Parser.Ops;

public sealed class WhileOperation : LoopOperation
{
  /// <summary>While Condition</summary>
  public required string? Condition { get; init; }
  protected override IOperation ContinueOp => new OperationContinue(LoopIndex, 0, CursorKey);
  protected override IOperation StartLoop => Condition is null
        ? throw Err.ThrowBadDef("Condition is null on a while loop")
        : new OperationCheckLoopCondition(Condition, CursorKey, CurrentIndex);

  protected override void Execute ()
  {
    Data[CursorKey] = new CursorData(Parser);
    Parser.SetNextOperationIndex(LoopIndex);
  }
}
