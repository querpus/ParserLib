#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  public Action<NodeContext, Token> Execute { get; init; } = (_, token) => Debug.Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Group}'", "NodeRule");
  public static NodeRule IgnoreGroup (string groupName, bool addToOrigin = false) => new()
  {
    TokenName = groupName,
    Execute = (context, token) =>
    {
      if (context.HasCurrent && addToOrigin)
      {
        context.Current.Origin += token.Value;
      }
    }
  };
}
