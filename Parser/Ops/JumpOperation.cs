namespace Parser.Ops;

public class JumpOperation : Operation
{
  public int TargetIndex { get; init; } = DNE;
  public string? TargetIndexVar { get; init; }
  public string? TargetLabel { get; init; }

  protected override void Execute ()
  {
    int index = DNE;

    if (TargetLabel is not null)
    {
      index = Parser.Labels[TargetLabel];
    }
    else if (TargetIndex is not DNE)
    {
      index = TargetIndex;
    }
    else if (TargetIndexVar is not null)
    {
      index = (int) Data[TargetIndexVar];
    }
    else
    {
      Err.ThrowBadDef("Required property not set.");
    }

    Parser.SetNextOperationIndex(index);
  }
}
