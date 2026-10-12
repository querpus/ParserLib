namespace Parser.Ops.Text;

/// <summary>Tokenizes a string or list of strings.</summary>
public class TokenizeOperation : Operation
{
  protected TokenRuleCollection Rules { get; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  public TokenizeOperation (IEnumerable<TokenRule> rules) => Rules = [.. rules];
  public TokenizeOperation () => Rules = [];

  protected override void Execute ()
  {
    if (Data[InputKey] is string input)
    {
      TokenFactory factory = new(Spec, Rules.Count == 0 ? null : Rules);
      Data[OutputKey] = factory.Tokenize(input);
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("string", Data[InputKey].TypeName);
    }
  }
}
