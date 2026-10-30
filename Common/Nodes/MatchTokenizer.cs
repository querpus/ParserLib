#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

namespace Common.Nodes;
/// <summary>Creates a token for every match with the groups as properties within every match. Multiple captures are stored as a Collection under the group name.</summary>
public class MatchTokenizer : ITokenizer<MatchToken>
{
  IEnumerable<MatchToken> ITokenizer<MatchToken>.Tokenize (string input) => Tokenize(input);
  IEnumerable<MatchToken> ITokenizer<MatchToken>.Tokens => Tokens;
  public const string TokenTypePrefix = "_t_";
  public Collection<MatchToken> Tokens { get; } = [];
  public dynamic? Rules { get; init; }
  public int Count => Tokens.Count;
  public void Reset () => Tokens.Clear();
  public Collection<MatchToken> Tokenize (string input)
  {
    Reset();
    if (Rules is null)
    {
      throw new InvalidOperationException("No token rules.");
    }

    TokenRuleSet rules;

    if  (Rules is IHasTokenRuleSet ihtrs)
    {
      rules = ihtrs.TokenRuleSet;
    }
    else if (Rules is TokenRuleSet)
    {
      rules = Rules;
    }
    else
    {
      throw new InvalidOperationException("Invalid token rules.");
    }

    if (Rules.TokenSequences.Count == 0)
    {
      throw new InvalidOperationException("No token sequences.");
    }

    var regex = Rules.Regex;
    int matchindex = 0;
    foreach (Match match in regex.Matches(input))
    {
      Dictionary<string, dynamic> groups = [];
      foreach (string groupName in regex.GetGroupNames())
      {
        Group group = match.Groups[groupName];
        if (group.Success && !groupName.Equals("0", SCO))
        {
          Collection<string> caps = [];
          for (int i = 0; i < group.Captures.Count; i++)
          {
            Capture cap = group.Captures[i];
            caps.Add(cap.Value);
          }
          if (caps.Count > 1)
          {
            groups.Add(groupName, caps);
          }
          else if (caps.Count == 1)
          {
            groups.Add(groupName, caps[0]);
          }
          else
          {
            // Do not add groups if they have no captures.
          }
        }
      }
      string name = SE;
      if (groups.Any(g => g.Key.StartsWith(TokenTypePrefix, SCOIC)))
      {
        name = groups.First(g => g.Key.StartsWith(TokenTypePrefix, SCOIC)).Value;
      }
      var token = new MatchToken()
      {
        Name = name,
        MatchIndex = matchindex,
        Position = new(match.Index, match.Index + match.Length),
        GroupData = groups,
        Value = match.Value
      };
      Tokens.Add(token);
      matchindex++;
    }
    return Tokens;
  }
}
