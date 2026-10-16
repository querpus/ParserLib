#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

using static Common.Debug;

namespace Common.NodeTree;

/// <summary>Creates a token for every capture with the group name as the type within every match. Multiple captures are additional tokens.</summary>
public class CaptureTokenizer : ITokenizer<CaptureToken>
{
  public Collection<CaptureToken> Tokens { get; } = [];
  public required TokenRuleSet Rules { get; init; }
  public int Count => Tokens.Count;
  public void Reset () => Tokens.Clear();
  public Collection<CaptureToken> Tokenize (string input)
  {
    Reset();

    if (Rules.TokenSequences.Count == 0)
    {
      throw new InvalidOperationException("No token sequences.");
    }

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
              Group = groupName,
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
