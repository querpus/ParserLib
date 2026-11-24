//using Parser.Text.Tokens;

namespace Parser.Ops.Text;

public class ExtractOperation : Operation
{
  public required string Pattern { get; init; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  public required string ExtractedKey { get; init; }

  protected override void Execute ()
  {
    Collection<string> extracted = [];

    string extract (string input)
    {
      foreach (Match match in Regex.Matches(input, Pattern))
      {
        extracted.Add(match.Value);
        input = input.Remove(match.Index, match.Length);
      }

      return input;
    }

    if (Data[InputKey] is string s)
    {
      Data[OutputKey] = extract(s);
    }
    else if (Data[InputKey] is IEnumerable<string> list)
    {
      Collection<string> modded_values = [];
      foreach (string item in list)
      {
        modded_values.Add(extract(item));
      }
      Data[OutputKey] = modded_values;
    }
    else
    {
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
    }

    Data[ExtractedKey] = extracted;
    Status = OpStatus.Pass;
  }
}
