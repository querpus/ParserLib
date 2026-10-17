#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;

public class TokenProcessor (IEnumerable<CaptureToken> tokens, NodeRuleSet ruleset)
{
  public Collection<CaptureToken> Tokens { get; } = [.. tokens];
  public NodeRuleSet RuleSet { get; } = ruleset;
  public NodeContext? Context { get; private set; }
  public void Process ()
  {
    Context = RuleSet.InitialSetup();
    int count = 0;
    foreach (CaptureToken token in tokens.Where(t => t.Name is not null).Order())
    {
      if (!RuleSet.RulesByGroup.TryGetValue(token.Name!, out NodeRule? rule))
      {
        Debug.Log(MsgClass.Warning, $"Token ID ({token.Name}) is not handled.", this);
        continue;
      }
      rule.Execute(Context, token);
      count++;
    }
    Debug.Log(MsgClass.Debug, $"Token Processing Complete: {count} tokens processed.", this);
  }
}
