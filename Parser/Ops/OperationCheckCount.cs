namespace Parser.Ops;

public sealed class OperationCheckCount : Operation
{
  public int BreakTarget { get; init; }
  public string CursorKey { get; init; }

  public OperationCheckCount (string cursor_key, int break_target)
  {
    BreakTarget = break_target;
    CursorKey = cursor_key;
  }
  public OperationCheckCount ()
  {
    BreakTarget = -1;
    CursorKey = SE;
  }

  protected override void Execute ()
  {
    if (Data.GetCursorByKey(CursorKey).AtEnd)
    {
      Parser.SetNextOperationIndex(BreakTarget);
      _ = Data.Remove(CursorKey);
      Status = OpStatus.LoopBreak;
    }
    else
    {
      Status = OpStatus.ConditionPass;
    }
  }
}
