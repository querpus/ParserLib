#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Token = Common.Entities.EToken;

namespace Common.NodeTree;

public static class StandardRuleSets
{
  private static void WarnIfCurrentHasData (NodeContext context)
  {
    if (context.Current is not null)
    {
      Debug.Log(MsgClass.Warning, "Current was not null before assignment.", "StandardRuleSets");
    }
  }
  private static void ErrorIfNull ([NotNull] object? current)
  {
    if (current is null)
    {
      Debug.Log(MsgClass.Error, "Node was null. Cannot Add Content.", "StandardRuleSets");
      throw new InvalidOperationException("Node was null. Cannot Add Content.");
    }
  }

  internal static NodeRule _element_open = new() {
    TokenName = "element_open",
    Execute = (context, token) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = new XMLElementNode() { Origin = token.Value };
      Debug.Log(MsgClass.GreenInfo, "Element Open Tag Open Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _element_close = new() {
    TokenName = "element_close",
    Execute = (context, _) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = context.NodeStack.Pop();
      Debug.Log(MsgClass.GreenInfo, "Beginning Close Validation", "StandardRuleSets");
    }
  };
  internal static NodeRule _element_single = new() {
    TokenName = "element_single",
    Execute = (context, token) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = new XMLElementNode() { Origin = token.Value };
      Debug.Log(MsgClass.GreenInfo, "Element Tag Single Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _tag_name = new() {
    TokenName = "tag_name",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);
      context.Current.AddData("Name", token.Value);
      Debug.Log(MsgClass.GreenInfo, "Adding Name to Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _a_name = new() {
    TokenName = "a_name",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);
      context.PropKey = token.Value;
    }
  };
  internal static NodeRule _a_value = new() {
    TokenName = "a_value",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);
      ErrorIfNull(context.PropKey);
      if (context.Current is XMLElementNode xen)
        xen.Attributes.Add(context.PropKey, new QString(token.Value));
    }
  };

  public static NodeRuleSet XMLRuleSet { get; } = new()
  {
    InitialSetup = () => new(),
    Rules = {
      _element_open,
      _element_close,
      _tag_name,
      _element_single,
      _a_name,
      _a_value
    },
  };
}

public class NodeContext
{
  public string? PropKey { get; set; }
  public Stack<INode> NodeStack { get; init; } = [];
  public INode? Current { get; set; }

  public bool IsRoot => NodeStack.Count == 1;
  public bool CanPop => NodeStack.Count > 0;
}

public class NodeRuleSet
{
  public required Func<NodeContext> InitialSetup { get; init; }
  public Collection<NodeRule> Rules { get; init; } = [];
  public Dictionary<string, NodeRule> RulesByGroup => Rules.Where(rule => rule.TokenName is not null).ToDictionary(rule => rule.TokenName!, StringComparer.OrdinalIgnoreCase);
}
public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  public Action<NodeContext, Token> Execute { get; init; } = (_, token) => Debug.Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Group}'", "NodeRule");
}

public class TokenProcessor (IEnumerable<Token> tokens, NodeRuleSet ruleset)
{
  public Collection<Token> Tokens { get; } = [.. tokens];
  public NodeRuleSet RuleSet { get; } = ruleset;
  public void Process ()
  {
    NodeContext context = RuleSet.InitialSetup();
    int count = 0;
    foreach (Token token in tokens.Where(t => t.Group is not null))
    {
      NodeRule rule = RuleSet.RulesByGroup[token.Group!];

      rule.Execute(context, token);
      count++;
      Debug.Log(MsgClass.Debug, $"Processed Token {token}", this);
    }
    Debug.Log(MsgClass.Debug, $"Token Processing Complete: {count} tokens processed.", this);
  }
}
