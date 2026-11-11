#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles



namespace Parser.Nodes;
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

    TokenRuleSet? rules =
      Rules is IHasTokenRuleSet ihtrs
      ? ihtrs.TokenRuleSet
      : Rules is TokenRuleSet set
        ? set
        : throw new InvalidOperationException("Invalid token rules.");

    if (rules is null || rules.TokenSequences.Count == 0)
    {
      throw new InvalidOperationException("No token rules.");
    }

    var regex = rules.Regex;
    int matchindex = 0;
    foreach (Match match in regex.Matches(input))
    {
      Dictionary<string, OneOrMany<string>> groups = [];
      string[] names = regex.GetGroupNames();
      foreach (string groupName in names)
      {
        Group group = match.Groups[groupName];
        if (group.Success && !groupName.Equals("0", SCO))
        {
          Collection<string> caps = [.. group.Captures.Select(c => c.Value)];

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
      string name = names.FirstOrDefault(g => g.StartsWith(TokenTypePrefix, SCOIC), SE);
      if (name.IsNotEmpty)
      {
        name = groups[name].Value ?? throw new InvalidTokenException("No suitable token name found.");
      }
      var token = new MatchToken()
      {
        Name = name ?? SE,
        MatchIndex = matchindex,
        Index = match.Index,
        GroupData = groups,
        Value = match.Value
      };
      Tokens.Add(token);
      matchindex++;
    }
    return Tokens;
  }
}
