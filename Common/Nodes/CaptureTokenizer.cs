#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

using static Common.Debug;

namespace Common.Nodes;

public interface ITokenizer<TToken>
{
  Collection<TToken> Tokens { get; }
  Collection<TToken> Tokenize (string input);
  int Count => Tokens.Count;
}

/// <summary>Creates a token for every match with the groups as properties within every match. Multiple captures are stored as a Collection under the group name.</summary>
public class MatchTokenizer : ITokenizer<MatchToken>
{
  public const string TokenTypePrefix = "_t_";
  public Collection<MatchToken> Tokens { get; } = [];
  public required TokenRuleSet Rules { get; init; }
  public int Count => Tokens.Count;
  public void Reset () => Tokens.Clear();
  public Collection<MatchToken> Tokenize (string input)
  {
    Reset();

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

/// <summary>Creates a token for every group in each match. Multiple captures are stored as an array of string data.</summary>
public class GroupTokenizer : ITokenizer<CaptureToken>
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
