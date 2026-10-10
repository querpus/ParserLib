#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

using static Common.Debug;

namespace Common.NodeTree;

public class NewTokenizer
{
  public Collection<EToken> Tokens { get; } = [];
  public required TokenRuleSet Rules { get; init; }
  protected int CurrentIndex { get; set; }

  public Collection<EToken> Tokenize (string input)
  {
    Tokens.Clear();
    CurrentIndex = 0;

    if (Rules.RegexString is null)
    {
      throw new InvalidOperationException("ParsingInfo.RegexString is null.");
    }

    var regex = Rules.Regex;
    foreach (Match match in regex.Matches(input))
    {
      foreach (string groupName in regex.GetGroupNames())
      {
        Group group = match.Groups[groupName];
        if (group.Success && !groupName.Equals("0", SCO))
        {
          EToken token = new()
          {
            Value = group.Value,
            Group = groupName,
            CaptureIndex = group.Captures.Count - 1,
            Position = group.Index..(group.Index + group.Length)
          };
          Tokens.Add(token);
          Log(MsgClass.GreenInfo, $"{token}", this);
        }
      }
    }
    return Tokens;
  }
}
