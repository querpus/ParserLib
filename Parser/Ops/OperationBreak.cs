namespace Parser.Ops;

public sealed class OperationBreak : Operation
{
  /// <summary>Created during parsing as a placeholder until unpacking.</summary>
  public static OperationBreak Null { get; } = new(-1, null!);
  public OperationBreak () { }
  [SetsRequiredMembers]
  public OperationBreak (int breakTarget, string breakCursor)
  {
    BreakTarget = breakTarget;
    BreakCursor = breakCursor;
  }

  /// <summary>The position to break to.</summary>
  /// <remarks>This will usually be the position after the original loop statement.</remarks>
  public required int BreakTarget { get; init; }
  /// <summary>The key storing the cursor for the current loop.</summary>
  public required string? BreakCursor { get; init; }

  protected override void Execute ()
  {
    Status = OpStatus.Pass;
    Parser.SetNextOperationIndex(BreakTarget);
    _ = BreakCursor is not null && Data.Remove(BreakCursor);
  }
}
