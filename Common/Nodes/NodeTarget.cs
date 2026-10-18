#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
/// <summary>The node to be designated a target or referred to as a source.</summary>
/// <remarks>You can use the 'OR' operator to specify a field within a node, or to make multiple assignments.</remarks>
[Flags]
public enum NodeTarget
{
  /// <summary>No target, do not store in a field.</summary>
  /// <remarks>This will throw an exception if you specify this as a source node.</remarks>
  Null = 0,
  /// <summary>This node's parent. This is the top of the node stack.</summary>
  /// <remarks>This field cannot be assigned. Use Push and Pop to travel up or down the hierarchy.</remarks>
  Parent = 1,
  /// <summary>The currently selected node.</summary>
  Current = 2,
  /// <summary>The document header field.</summary>
  Header = 4,
  /// <summary>The root document field.</summary>
  /// <remarks>Use this if the document requires a single root node.</remarks>
  Root = 8,
  /// <summary>The root document collection.</summary>
  /// <remarks>Use this if the document doesn't require a single root node.</remarks>
  RootNodes = 16,
  /// <summary>Write to parent, if the parent doesn't exist, write to root or add to rootnodes.</summary>
  /// <remarks>This has special logic when adding a node, it only assigns once, not to all of the target locations.</remarks>
  AnyRootOrParent = Parent | Root | RootNodes,
  /// <summary>The property name field.</summary>
  /// <remarks>This is a <see langword="string"/> field.</remarks>
  PropKey = 32,
  /// <summary>The token group (aka. Token Type or Token Name)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenGroup = 64,
  /// <summary>The token value (aka. Token Content)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenValue = 128,
  /// <summary>Access the Prefix field of a node.</summary>
  /// <remarks>Read only. This is a <see langword="string"/> field that must be modified with the <see cref="INode.AppendToPrefix(string)"/> method or its overloads.</remarks>
  Prefix = 256,
  /// <summary>Access the Suffix field of a node.</summary>
  /// <remarks>Read only. This is a <see langword="string"/> field that must be modified with the <see cref="INode.AppendToSuffix(string)"/> method or its overloads.</remarks>
  Suffix = 512,
  /// <summary>The readonly fields that cannot be assigned to.</summary>
  Fields = TokenGroup | TokenValue | Prefix | Suffix,
}
