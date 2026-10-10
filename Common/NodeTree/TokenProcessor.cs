#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public class TokenProcessor (IEnumerable<Token> tokens, NodeRuleSet ruleset)
{
  public Collection<Token> Tokens { get; } = [.. tokens];
  public NodeRuleSet RuleSet { get; } = ruleset;
  public NodeContext? Context { get; private set; }
  public void Process ()
  {
    Context = RuleSet.InitialSetup();
    int count = 0;
    Collection<Token> finalTokens = [.. tokens.Where(t => t.Group is not null).Order()];
    foreach (Token token in finalTokens)
    {
      if (!RuleSet.RulesByGroup.TryGetValue(token.Group!, out NodeRule? rule))
      {
        Debug.Log(MsgClass.Warning, $"Token ID ({token.Group}) is not handled.", this);
        continue;
      }
      rule.Execute(Context, token);
      count++;
      Debug.Log(MsgClass.Debug, $"Processed Token {token}", this);
    }
    Debug.Log(MsgClass.Debug, $"Token Processing Complete: {count} tokens processed.", this);
  }
}
