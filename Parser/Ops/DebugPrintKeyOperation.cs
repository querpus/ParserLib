namespace Parser.Ops;

/// <summary>Prints a key that implements <see cref="IPrintable"/>.</summary>
/// <remarks>If the key does not implement <see cref="IPrintable"/>, nothing happens, and the operation returns <see cref="OpStatus.Skipped"/>.</remarks>
public class DebugPrintKeyOperation : Operation
{
  /// <summary>The key to print.</summary>
  public required string? InputKey { get; init; }
  /// <inheritdoc/>
  /// <remarks>This is a debugging operation and does not store data.</remarks>
  protected override void Execute ()
  {
    if (Data[InputKey] is IPrintable printable)
    {
      printable.Print();
      NewLine();
      Status = OpStatus.Pass;
    }
    else
    {
      Status = OpStatus.Skipped;
    }
  }

  public override string ToString () => $"DebugPrintKeyOperation Key = \"{InputKey}\"";
}
