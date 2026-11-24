namespace Parser.Ops.Text;

public class RemoveCommentsOperation ([SS("Regex")] string comment, [SS("Regex")] string quote, string replaceWith = "") : Operation
{
  [SS("Regex")]
  protected string Assembled => $"(?<_comment>{comment})|(?<_quote>{quote})";
  protected Regex OpRegex => new(Assembled);
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }

  private string Task (string s) => s.ReplaceAllIfContainsGroup(OpRegex.Matches(s), "_comment", replaceWith);

  protected override void Execute ()
  {
    Status = OpStatus.Pass;
    Data[OutputKey] =
      Data[InputKey] is string s ?
        Task(s) :
      Data[InputKey] is IEnumerable<string> list ?
        list.Select(Task) :
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
  }
}
