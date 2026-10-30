#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

using static Common.Debug;

namespace Common.Nodes;

/// <summary>Creates a token for every group in each match. Multiple captures are stored as an array of string data.</summary>
public class GroupTokenizer : ITokenizer<CaptureToken>
{
  IEnumerable<CaptureToken> ITokenizer<CaptureToken>.Tokenize (string input) => Tokenize(input);
  IEnumerable<CaptureToken> ITokenizer<CaptureToken>.Tokens => Tokens;
  public Collection<CaptureToken> Tokens { get; } = [];
  public dynamic? Rules { get; init; }
  public int Count => Tokens.Count;

  public void Reset () => Tokens.Clear();
  public Collection<CaptureToken> Tokenize (string input)
  {
    Reset();
    if (Rules is null)
      throw new InvalidOperationException("No rules.");
    if (Rules.TokenSequences.Count == 0)
      throw new InvalidOperationException("No token sequences.");

    var regex = Rules.Regex;
    foreach (Match match in regex.Matches(input))
    {
      foreach (string groupName in regex.GetGroupNames())
      {
        Group group = match.Groups[groupName];
        if (group.Success && !groupName.Equals("0", SCO))
        {
          for (int i = 0; i < group.Captures.Count; i++)
          {
            Capture cap = group.Captures[i];
            CaptureToken token = new()
            {
              Value = cap.Value,
              Name = groupName,
              CaptureIndex = i,
              Position = cap.Index..(cap.Index + cap.Length)
            };
            Tokens.Add(token);
            Log(MsgClass.GreenInfo, $"{token}", this);
          }
        }
      }
    }
    return Tokens;
  }
}
