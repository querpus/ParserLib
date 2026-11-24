#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;

public class CaptureTokenProcessor : INodeProcessor
{
  private readonly IEnumerable<IToken> _tokens;
  public CaptureTokenProcessor (IEnumerable<IToken> tokens, NodeRuleSet ruleset)
  {
    _tokens = tokens;
    Input = tokens;
    Tokens = [.. tokens.Order()];
    RuleSet = ruleset;
  }

  public dynamic? Input { get; init; }
  public IEnumerable<IToken> Tokens { get; }
  public NodeRuleSet RuleSet { get; }
  public NodeContext? Context { get; private set; }
  public void Process ()
  {
    var start_time = DateTime.Now;
    Context = RuleSet.InitialSetup();
    int count = 0;
    foreach (IToken token in _tokens.Order())
    {
      if (!RuleSet.TryGetRule(token, out NodeRule? rule))
      {
        Log(MsgClass.Warning, $"Token [{token.Type}] '{token.Content}' was not handled.", this);
        continue;
      }
      Context.SetToken(token);
      rule.Execute(Context);
      Context.ClearTemp();
      count++;
    }
    var end_time = DateTime.Now;
    TimeSpan duration = end_time - start_time;
    Log(MsgClass.Debug, $"Token Processing Complete: {count} tokens processed in {duration.TotalSeconds}s.", this);
  }
}
