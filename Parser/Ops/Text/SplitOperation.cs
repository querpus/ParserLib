#pragma warning disable IDE0018 // Inline variable declaration

namespace Parser.Ops.Text;

/// <summary>
/// Takes an input as a <see langword="string"/> or a collection of strings,
/// and splits the contents into a single <see cref="Collection{T}"/> of strings.
/// This will correctly handle a <see cref="Collection{T}"/> of strings, further spliting them
/// and flattening the resulting list to a standard <see cref="Collection{T}"/>.
/// </summary>
/// <remarks><br/>
/// <h4>Allowed Input Types</h4>
/// <see langword="string"/>, <see cref="IEnumerable{T}"/>(<see langword="string"/>)<br/>
/// <h4>Produced Output Types</h4>
/// <see cref="Collection{T}"/>(<see langword="string"/>)<br/>
/// </remarks>
public class SplitOperation : Operation
{
  #region Private Members
  private RegexOptions RXOptions { get; set; } = RON;
  #endregion
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  public string? Procedure { get; init; }
  public IEnumerable<string> Delims { get; init; } = [];
  public IEnumerable<string> RegexOptions { get; init; } = [];

  #region Constructors
  public SplitOperation ()
  {

  }
  [SetsRequiredMembers]
  public SplitOperation (string delimeter, string input_key, string output_key)
  {
    Procedure = "Delim";
    Delims = [delimeter];
    InputKey = input_key;
    OutputKey = output_key;
  }
  [SetsRequiredMembers]
  public SplitOperation (IEnumerable<string> delimeters, string input_key = "text", string output_key = "textparts")
  {
    Procedure = "Delim";
    Delims = [.. delimeters];
    InputKey = input_key;
    OutputKey = output_key;
  }
  [SetsRequiredMembers]
  public SplitOperation ([SS("regex")] string regex, RegexOptions regex_options, string input_key = "text", string output_key = "textparts")
  {
    Procedure = "Regex";
    Delims = [regex];
    RXOptions = regex_options;
    InputKey = input_key;
    OutputKey = output_key;
  }
  [SetsRequiredMembers]
  public SplitOperation (RxSCollection regexes, RegexOptions regex_options, string input_key = "text", string output_key = "textparts")
  {
    Procedure = "Regex";
    Delims = [.. regexes];
    RXOptions = regex_options;
    InputKey = input_key;
    OutputKey = output_key;
  }
  [SetsRequiredMembers]
  public SplitOperation (string input_key = "text", string output_key = "textparts")
  {
    Procedure = "None";
    InputKey = input_key;
    OutputKey = output_key;
  }
  #endregion Constructors
  protected override void Execute ()
  {
    foreach (string opt in RegexOptions)
    {
      RXOptions |= opt.ToUpperInvariant() switch
      {
        "CASEINSENSITIVE" => ROIC,
        "RIGHTTOLEFT" => ROR2L,
        "EXPLICITCAPTURE" => ROEC,
        "MULTILINE" => ROML,
        "DOTMATCHESNEWLINE" => ROSL,
        "IGNORECASE" => ROIC,
        "SINGLELINE" => ROSL,
        "IGNOREPATTERNWHITESPACE" => ROIPW,
        "NONE" => RON,
        "NONBACKTRACKING" => RONB,
        _ => RON,
      };
    }
    IEnumerable<string> delimSplit (string input) => input.Split([.. Delims ?? []], SSORT);

    Data[OutputKey] = Procedure switch
    {
      "None" when Data[InputKey] is string str => RX.LineEnd.Split(str),
      "Delim" when Data[InputKey] is string str => delimSplit(str),
      "Delim" when Data[InputKey] is IEnumerable<string> list => list.SelectMany(delimSplit),
      "Regex" when Data[InputKey] is string str => new Regex((Delims ?? []).TextJoin("|"), RXOptions).Split(str),
      "Regex" when Data[InputKey] is IEnumerable<string> list => list.SelectMany(str => new Regex((Delims ?? []).TextJoin("|"), RXOptions).Split(str)),
      _ => Err.ThrowBadInput("string or list", Data[InputKey].TypeName),
    };

    Status = OpStatus.Pass;
  }
}
