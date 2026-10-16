#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

[Flags]
public enum NodeTarget
{
  /// <summary>No target, do not store in a field.</summary>
  Null = 0,
  /// <summary>This node's parent.</summary>
  /// <remarks>Use Push and Pop to travel up or down the hierarchy.</remarks>
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
  /// <summary>The property name field.</summary>
  /// <remarks>This is a <see langword="string"/> field.</remarks>
  PropKey = 32,
  /// <summary>The token group (aka. Token Type or Token Name)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenGroup = 64,
  /// <summary>The token value (aka. Token Content)</summary>
  /// <remarks>Read only. Cannot write to token. This is a <see langword="string"/> field.</remarks>
  TokenValue = 128,
  /// <summary>Write to parent, if the parent doesn't exist, write to root or add to rootnodes.</summary>
  AnyRootOrParent = Parent | Root | RootNodes,
}
