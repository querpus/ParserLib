using Parser.Condition;

namespace Parser.Ops;

/// <summary>This is placed at the start of a while loop.</summary>
/// <param name="condition">The condition to check.</param>
/// <param name="cursor_key">The key for the cursor.</param>
/// <param name="break_target">The target index to break to.</param>
public sealed class OperationCheckLoopCondition (string condition, string cursor_key, int break_target) : Operation
{
  public int BreakTarget { get; init; } = break_target;
  public string CursorKey { get; init; } = cursor_key;
  public ParsedExpression Condition { get; init; } = (ParsedExpression) condition;

  protected override void Execute ()
  {
    if (Condition.Evaluate(Data) is bool b && b)
    {
      Status = OpStatus.ConditionPass;
    }
    else
    {
      Parser.SetNextOperationIndex(BreakTarget);
      _ = Data.Remove(CursorKey);
      Status = OpStatus.ConditionFail;
    }
  }
}
