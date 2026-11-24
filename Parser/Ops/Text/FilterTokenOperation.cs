namespace Parser.Ops.Text;

/// <summary>Represents an operation to filter tokens based on specified criteria.</summary>
public class FilterTokenOperation : Operation
{
  public required string StrType { get; init; }
  public string? OpData { get; init; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }

  private FilterTokenType Type => Enum.Parse<FilterTokenType>(StrType, true);
  private enum FilterTokenType
  {
    None = 0,
    Empty = 1,
    Whitespace = 2,
    TokenType = 3,
    MatchEntireToken = 4,
    AnyMatchInToken = 5
  }
  [SetsRequiredMembers]
  public FilterTokenOperation (string input_key, string output_key, [StringSyntax("regex")] string rx, bool accept_any_match)
  {
    InputKey = input_key;
    OutputKey = output_key;
    StrType = accept_any_match ? "AnyMatchInToken" : "MatchEntireToken";
    OpData = rx;
  }
  [SetsRequiredMembers]
  public FilterTokenOperation (string input_key, string output_key, object token_type)
  {
    InputKey = input_key;
    OutputKey = output_key;
    StrType = "TokenType";
    OpData = token_type.ToString();
  }
  [SetsRequiredMembers]
  public FilterTokenOperation (string input_key, string output_key, bool only_remove_empty_tokens)
  {
    InputKey = input_key;
    OutputKey = output_key;
    StrType = only_remove_empty_tokens ? "Empty" : "Whitespace";
  }
  public FilterTokenOperation () { }
  protected override void Execute ()
  {
    if (Data[InputKey] is IEnumerable<IToken> tc)
    {
      Data[OutputKey] = Type switch
      {
        FilterTokenType.Empty => [.. tc.Where(tok => tok.Content.IsNotEmpty)],
        FilterTokenType.Whitespace => [.. tc.Where(tok => !tok.Content.IsWhitespace)],
        FilterTokenType.AnyMatchInToken when OpData is not null => [.. tc.Where(tok => tok is Token && !Regex.IsMatch(tok.Content, OpData))],
        FilterTokenType.MatchEntireToken when OpData is not null => [.. tc.Where(tok => tok is Token && Regex.Match(tok.Content, OpData).Length != tok.Content.Length)],
        FilterTokenType.TokenType => [.. tc.Where(tok => !tok.Type.Like(OpData))],
        FilterTokenType.None => [.. tc],
        _ => (TokenCollection) Err.ThrowBadDef("Invalid Filter Parameters.")
      };
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("IEnumerable<IToken>", Data[InputKey].TypeName);
    }
  }
}
