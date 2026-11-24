//using Parser.Text.Tokens;

namespace Parser.Ops.Text;

public class BetweenRegexOperation : Operation
{
  protected RxS Assembled => new(@$"({Prefix})(?<keep>[\s\S]*?)({Suffix})");
  protected Regex OpRegex => new(Assembled, ROEC | Spec.RxOpt);
  [SS("Regex")]
  public required string Prefix { get; init; }
  [SS("Regex")]
  public required string Suffix { get; init; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  private Collection<string> DoTask (string input) =>
    [.. from item in OpRegex.Matches(input) select item.Groups["keep"].Value];
  protected override void Execute ()
  {
    Data[OutputKey] =
      Data[InputKey] is string s ?
        DoTask(s) :
      Data[InputKey] is IEnumerable<string> list ?
        list.Select(DoTask) :
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);

    Status = OpStatus.Pass;
  }
}
