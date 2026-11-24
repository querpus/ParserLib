namespace Parser.Ops.Text;
/// <summary>
/// Takes an input as a <see cref="TokenCollection"/>,
/// and assembles the tokens into structures defined by rules.<br/>
/// Internally uses a <see cref="TokenAssembler"/> to do the work.
/// </summary>
/// <remarks><code>
/// InputKey: <see cref="TokenCollection"/>, <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IToken"/>&gt;<br/>
/// OutputKey: <see cref="TokenCollection"/></code>
/// <br/>
/// </remarks>
public class TokenAssembleOperation : Operation
{
  public IEnumerable<TokenRule> Rules { get; set; } = [];
  public required string OutputKey { get; init; }
  public required string InputKey { get; init; }
  public TokenAssembleOperation () { }
  [SetsRequiredMembers]
  public TokenAssembleOperation (IEnumerable<TokenRule> rules, string input_key = "tokens", string output_key = "tokens_assembled")
  {
    Rules = [.. rules];
    InputKey = input_key;
    OutputKey = output_key;
  }
  protected override void Execute ()
  {
    TokenAssembler assembler = Rules.IsEmpty ? new(Spec) : new([.. Rules], Spec);
    if (Data[InputKey] is IEnumerable<IToken> tc)
    {
      Data[OutputKey] = assembler.Execute(tc);
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("TokenCollection", Data[InputKey].TypeName);
    }
  }
}
