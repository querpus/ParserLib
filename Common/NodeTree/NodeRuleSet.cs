#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public class NodeRuleSet
{
  public required Func<NodeContext> InitialSetup { get; init; }
  public Collection<NodeRule> Rules { get; init; } = [];
  public Dictionary<string, NodeRule> RulesByGroup => Rules.Where(rule => rule.TokenName is not null).ToDictionary(rule => rule.TokenName!, StringComparer.OrdinalIgnoreCase);
}
