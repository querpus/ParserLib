namespace Parser.Ops;

/// <summary>Iterates over the specified key, storing the current item in another key to be accessed.</summary>
public sealed class ForEachOperation : LoopOperation
{
  public required string ListKey { get; init; }
  protected override IOperation ContinueOp => new OperationContinue(LoopIndex, 1, CursorKey);
  protected override IOperation StartLoop => new OperationCheckCount(CursorKey, CurrentIndex);
  protected override void Execute ()
  {
    Data[CursorKey] = new CursorData(Parser, 0, ListKey);
    Parser.SetNextOperationIndex(LoopIndex);
  }
}
