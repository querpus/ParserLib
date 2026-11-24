namespace Parser.Ops;

/// <summary>This operation ends the operation sequence with a success.</summary>
public sealed class OperationEnd (bool fail = false) : Operation
{
  protected override void Execute ()
  {
    Parser.SetNextOperationIndex(-1);
    Status = fail ? OpStatus.DefinedFail : OpStatus.EndCommand;
    if (fail) throw new QuitException();
  }
}
