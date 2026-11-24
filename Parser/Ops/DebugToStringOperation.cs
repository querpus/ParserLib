namespace Parser.Ops;
/// <summary>An operation that logs the contents of the provided key.</summary>
public class DebugToStringOperation : Operation
{
  public required string InputKey { get; init; }

  /// <inheritdoc/>
  /// <remarks>This is a debugging operation and does not store data.</remarks>
  protected override void Execute ()
  {
    switch (Data[InputKey])
    {
      case string s:
        Log(MsgClass.Debug, s, this);
        break;
      case IEnumerable<string> strs:
        foreach (string str in strs)
          Log(MsgClass.Debug, str, this);
        break;
      case IEnumerable<IToken> itokens:
        foreach (IToken it in itokens)
          Log(MsgClass.Debug, it.ToString2() ?? "<null data>", this);
        break;
      default:
        Log(MsgClass.Debug, Data[InputKey]?.ToString2() ?? "<null data>", this);
        break;
    }
  }

  public override string ToString () => $"DebugToStringOperation Key = \"{InputKey}\"";
}
