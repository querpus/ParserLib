#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using static Parser.Nodes.NodeTarget;

namespace Parser.Nodes;
public static class NodeTargetExtensions
{
  extension (NodeTarget nt)
  {

    public NodeTarget[] GetTargets (NodeTarget inclusive_mask = None, NodeTarget exclusive_mask = None)
    {
      if (inclusive_mask > None)
        nt &= inclusive_mask;
      if (exclusive_mask > None)
        nt &= ~exclusive_mask;
      return [.. Enum.GetValues<NodeTarget>().Where(r => (nt & r) == r && r != None)];
    }

    /// <summary>Write to parent, if the parent doesn't exist, add to rootnodes.</summary>
    public static NodeTarget RootOrParent => Parent | Root;
    /// <summary>Properties that store a node or nodes for single-root documents.</summary>
    public static NodeTarget NodeProperties => Current | Parent | Header | Root | Temp;
    /// <summary>The readonly fields that nodes cannot be assigned to.</summary>
    public static NodeTarget Fields => TokenGroup | TokenValue | Context;
  }
}

/// <summary>The node to be designated a target or referred to as a source.</summary>
/// <remarks>You can use the 'OR' operator to specify a field within a node, or to make multiple assignments.</remarks>
[Flags]
public enum NodeTarget
{
  /// <summary>No target, do not store in a field.</summary>
  /// <remarks>This will throw an exception if you specify this as a source node.</remarks>
  None = 0,
  /// <summary>This node's parent. This is the top of the node stack.</summary>
  /// <remarks>This field cannot be assigned. Use Push and Pop to travel up or down the hierarchy.</remarks>
  Parent = 1,
  /// <summary>The currently selected node.</summary>
  Current = 1 << 1,
  /// <summary>The document header field.</summary>
  Header = 1 << 2,
  /// <summary>The root document field.</summary>
  /// <remarks>This references the <see cref="NodeContext.RootNodes"/> collection if <see cref="NodeContext.SingleRoot"/> is <see langword="false"/>.</remarks>
  Root = 1 << 3,
  /// <summary>This property is cleared after every node execution.</summary>
  /// <remarks>Does not persist between node executions.</remarks>
  Temp = 1 << 4,
  /// <summary>The token group (aka. Token Type or Token Name)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenGroup = 1 << 5,
  /// <summary>The token value (aka. Token Content)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenValue = 1 << 6,
  /// <summary>The current context</summary>
  /// <remarks>Read only. Cannot write to context, only to data keys within it. This is only to retrieve and store data in keys.</remarks>
  Context = 1 << 7,
}
