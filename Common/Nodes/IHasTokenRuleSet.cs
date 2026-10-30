#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public interface IHasTokenRuleSet
{
  TokenRuleSet? TokenRuleSet { get; }
}
