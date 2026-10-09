#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Token = Common.Entities.EToken;

namespace Common.NodeTree;

public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  public Action<NodeContext, Token> Execute { get; init; } = (_, token) => Debug.Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Group}'", "NodeRule");
}
