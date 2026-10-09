#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public class NodeContext
{
  public string? PropKey { get; set; }
  public Stack<INode> NodeStack { get; init; } = [];
  public INode? Current { get; set; }
  public INode? Root { get; set; }

  public bool IsRoot => NodeStack.Count == 0;
  public bool CanPop => NodeStack.Count > 0;
}
