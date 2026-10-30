#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public class TokenRuleSet
{
  public required ObservableCollection<string> TokenSequences { get; init; }
  public RegexOptions RegexOptions { get; init; } = ROIPW | ROML;
  public Regex Regex => new(TokenSequences.TextJoin("|"), RegexOptions);
}
