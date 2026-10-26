namespace Parser.Ops.Text;

public class TrimOperation : Operation
{
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  protected override void Execute ()
  {
    Status = OpStatus.Pass;
    Data[OutputKey] = Data[InputKey] switch
    {
      string s => s.Trim(),
      IEnumerable<string> ien => ien.Select(x => x.Trim()),
      _ => Data[InputKey]
    };

    if (Data[InputKey] is not string and not IEnumerable<string>)
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
  }
}
