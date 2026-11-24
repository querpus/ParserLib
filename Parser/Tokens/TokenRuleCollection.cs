#pragma warning disable CA1710 // Identifiers should have correct suffix
namespace Parser.Tokens;

public sealed class TokenRuleCollection : Collection<TokenRule>
{
  public void AddRange (IEnumerable<TokenRule> children) => IListExtensions.AddRange(this, children);
}
