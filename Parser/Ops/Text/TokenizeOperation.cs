using Common.Nodes;

namespace Parser.Ops.Text;

/// <summary>Tokenizes a string or list of strings.</summary>
public class TokenizeOperation : Operation
{
  protected TokenRuleCollection Rules { get; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  public string? Tokenizer { get; init; }
  public TokenizeOperation (IEnumerable<TokenRule> rules) => Rules = [.. rules];
  public TokenizeOperation () => Rules = [];

  protected override void Execute ()
  {
    if (Data[InputKey] is string input)
    {
      if (Tokenizer.Like(["xparser", "tokenfactory"]) || Tokenizer is null)
      {
        TokenFactory factory = new() { Spec = Spec };
        Data[OutputKey] = factory.Tokenize(input);
      }
      else if (Tokenizer.Like(["capture", "cap"]))
      {
        CaptureTokenizer ctokenizer = new() { Rules = Spec };
        Data[OutputKey] = ctokenizer.Tokenize(input);
      }
      else
      {
        throw Err.ThrowBadDef("Invalid value for tokenizer type.");
      }
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("string", Data[InputKey].TypeName);
    }
  }
}
