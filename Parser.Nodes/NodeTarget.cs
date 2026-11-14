#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using static Parser.Nodes.NodeTarget;
using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;
public static class NodeTargetExtensions
{
  extension (NT nt)
  {
    public string DataType => nt switch
    {
      None => "null",
      NT when nt.HasAnyFlag(TokenGroup | TokenValue) => "string",
      Context => "context"
    };
    public NT[] GetTargets (NT inclusive_mask = None, NT exclusive_mask = None)
    {
      if (inclusive_mask > None)
        nt &= inclusive_mask;
      if (exclusive_mask > None)
        nt &= ~exclusive_mask;
      return [.. Enum.GetValues<NT>().Where(r => (nt & r) == r && r != None)];
    }

    /// <summary>Write to parent, if the parent doesn't exist, add to rootnodes.</summary>
    public static NT RootOrParent => Parent | Root;
    /// <summary>Properties that store a node or nodes for single-root documents.</summary>
    public static NT NodeProperties => Current | Parent | Header | Root | Temp;
    /// <summary>The readonly fields that nodes cannot be assigned to.</summary>
    public static NT Fields => TokenGroup | TokenValue | Context;
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
  /// <summary>The currently read token group (aka. token type or token name)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenGroup = 1 << 5,
  /// <summary>The currently read token value (aka. Token Content)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenValue = 1 << 6,
  /// <summary>The current context</summary>
  /// <remarks>Read only. Cannot write to context, only to data keys within it. This is only to retrieve and store data in keys.</remarks>
  Context = 1 << 7,
  /// <summary>A modifier that indicates to look at the node's data.</summary>
  /// <remarks>This will always be coupled with an <see cref="INode"/> property.</remarks>
  Data = 1 << 8,
  /// <summary>A modifier that indicates to look at the node's children.</summary>
  /// <remarks>This will always be coupled with an <see cref="INode"/> property.<br/>
  /// <b>TKey</b> will always be a <see langword="string"/>, <b>TValue</b> could be anything except <see langword="null"/>.<br/>
  /// A <see langword="null"/> entry to <see cref="INode.Data"/> will clear the key.
  /// </remarks>
  Children = 1 << 9,
  /// <summary>A modifier that indicates to look at the node's properties.</summary>
  /// <example><c>Current|Properties</c> - The properties <see cref="Dictionary{TKey,TValue}"/> of the <see cref="NodeContext.Current"/> context property.</example>
  /// <remarks>
  /// This will always be coupled with an <see cref="INode"/> property.<br/>
  /// <b>TKey</b> will always be a <see langword="string"/>, <b>TValue</b> will always be a type of <see cref="INode"/>
  /// </remarks>
  Properties = 1 << 10,
}
