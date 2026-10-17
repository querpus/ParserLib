#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  public static void DefaultAction (NodeContext context, CaptureToken token) => Debug.Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Group}'", "NodeRule");
  public Action<NodeContext, CaptureToken> Execute { get; init; } = DefaultAction;
  public static NodeRule IgnoreGroup (string groupName, bool addToOriginHead = false, bool addToOriginTail = false) => new()
  {
    TokenName = groupName,
    Execute = (context, token) =>
    {
      if (context.HasCurrent && addToOriginHead)
      {
        context.Current.OriginHead += token.Value;
      }
      if (context.HasCurrent && addToOriginTail)
      {
        context.Current.OriginTail += token.Value;
      }
    }
  };
}
