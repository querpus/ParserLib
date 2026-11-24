namespace Parser.Ops;

public sealed class OperationContinue : Operation, IEquatable<OperationContinue>
{
  public static OperationContinue Null { get; } = new(-1, 0, null!);
  public required int Target { get; set; }
  public required string CursorKey { get; set; }
  public required int Increment { get; set; }

  [SetsRequiredMembers]
  public OperationContinue (int continue_target, int increment, string cursor_key)
  {
    Target = continue_target;
    CursorKey = cursor_key;
    Increment = increment;
  }
  public OperationContinue () { }

  protected override void Execute ()
  {
    if (Target == -1)
    {
      throw Err.ThrowBadDef("Continue operation not set up.");
    }

    Parser.SetNextOperationIndex(Target);

    if (CursorKey is not null && Increment != 0)
    {
      Data.IncCursorIndex(CursorKey, Increment);
    }
  }

  public bool Equals (OperationContinue? other) => Target.Equals(other?.Target) && CursorKey.Equals(other?.CursorKey, SCOIC) && Increment.Equals(other?.Increment);
  public override bool Equals (object? obj) => Equals(obj as OperationContinue);
  public override int GetHashCode () => HashCode.Combine(Target, CursorKey, Increment);
}
