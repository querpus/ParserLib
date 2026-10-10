#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public enum NodeTarget
{
  /// <summary>This node's parent.</summary>
  /// <remarks>Read only. Cannot write to this field.</remarks>
  Parent,
  Current,
  Header,
  Root,
  RootNodes,
  PropKey,
  /// <summary>The token group.</summary>
  /// <remarks>Read only. Cannot write to token.</remarks>
  TokenGroup,
  TokenValue,
}
