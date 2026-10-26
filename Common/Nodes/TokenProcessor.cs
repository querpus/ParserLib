#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Xml.Linq;

namespace Common.Nodes;

public interface INodeProcessor
{
  dynamic? Input { get; init; }
  /// <summary></summary>
  NodeContext? Context { get; }
  /// <summary>Processes and creates the <see cref="Context"/>.</summary>
  [MemberNotNull(nameof(Context))]
  void Process ();
  static NodeContext Process<TProcessor> (dynamic input) where TProcessor : INodeProcessor, new()
  {
    TProcessor engine = new() { Input = input };
    engine.Process();
    return engine.Context;
  }
}

public class XElementProcessor : INodeProcessor
{
  public NodeContext? Context { get; private set; }
  public dynamic? Input { get; init; }

  public XElementProcessor () { }
  public XElementProcessor (XElement root) => Input = root;

  public void Process ()
  {
    var start_time = DateTime.Now;
    Context = new() { SingleRoot = true, Root = GetNodes(Input) };
    var end_time = DateTime.Now;
    TimeSpan duration = end_time - start_time;
    Debug.Log(MsgClass.Debug, $"XElement Processing Complete: processed in {duration}.", this);
  }

  private static XMLElementNode GetNodes (XElement element)
  {
    XMLElementNode result = new();

    result.Data["Name"] = element.Name.LocalName;

    if (element.HasElements)
    {
      element.Elements().Foreach(c => result.Children.Add(GetNodes(c)));
    }
    if (element.HasAttributes)
    {
      foreach (var attr in element.Attributes())
      {
        string attr_name = attr.Name.LocalName;
        string attr_value = attr.Value;
        result.Attributes.Add(attr_name, new QString(attr_value));
      }
    }
    if (!element.Value.IsWhitespace && !element.Value.IsEmpty)
    {
      string value = element.Value;
      result.Children.Add(new XMLContentNode { Content = value });
    }
    return result;
  }
}

public class TokenProcessor (IEnumerable<IToken> tokens, NodeRuleSet ruleset) : INodeProcessor
{
  public dynamic? Input { get; init; } = tokens;
  public IEnumerable<IToken> Tokens { get; } = [.. tokens];
  public NodeRuleSet RuleSet { get; } = ruleset;
  public NodeContext? Context { get; private set; }
  public void Process ()
  {
    var start_time = DateTime.Now;
    Context = RuleSet.InitialSetup();
    int count = 0;
    foreach (IToken token in tokens.Where(t => t.Name is not null).Order())
    {
      if (!RuleSet.RulesByGroup.TryGetValue(token.Name!, out NodeRule? rule))
      {
        Debug.Log(MsgClass.Warning, $"Token ID ({token.Name}) is not handled.", this);
        continue;
      }
      rule.Execute(Context, token);
      count++;
    }
    var end_time = DateTime.Now;
    TimeSpan duration = end_time - start_time;
    Debug.Log(MsgClass.Debug, $"Token Processing Complete: {count} tokens processed in {duration}.", this);
  }
}
