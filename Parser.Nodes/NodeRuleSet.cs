#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;

public class NodeRuleSet
{
  public required Func<NodeContext> InitialSetup { get; init; }
  public ObservableCollection<NodeRule> Rules { get; init; } = [];

  public bool TryGetRule ([NotNull] in IToken token, [NotNullWhen(true)] out NodeRule? rule)
  {
    rule = null;
    foreach (var chk in Rules)
    {
      bool exact_match = chk.ExactText?.Is(token.Content) != false || (chk.ExactText.Like(token.Content) && chk.IgnoreCase);
      bool token_match = chk.TokenName?.Like(token.Type) != false;

      rule = exact_match && token_match ? chk : null;

      if (rule is not null)
        return true;
    }

    return false;
  }
}
