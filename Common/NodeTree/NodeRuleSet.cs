#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public class TokenRuleSet
{
  public required Collection<string> TokenSequences { get; init; }
  public string RegexString => TokenSequences.TextJoin("|");
  public RegexOptions RegexOptions { get; init; } = ROIPW | ROML;
  public Regex Regex => new(RegexString, RegexOptions);
}

public class NodeRuleSet
{
  public required Func<NodeContext> InitialSetup { get; init; }
  public Collection<NodeRule> Rules { get; init; } = [];
  public Dictionary<string, NodeRule> RulesByGroup => Rules.Where(rule => rule.TokenName is not null).ToDictionary(rule => rule.TokenName!, StringComparer.OrdinalIgnoreCase);
}
