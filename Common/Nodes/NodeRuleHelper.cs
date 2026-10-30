#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using NT = Common.Nodes.NodeTarget;

namespace Common.Nodes;

public static class NodeRuleHelper
{
  extension (string className)
  {
    public object? CreateClass () =>
      Type.GetType(className)?.InvokeMember(SE, BFCI, null, null, null, CIIC);
  }

  extension (string target)
  {
    public NT ToNodeTarget () => target switch
    {
      "Parent" => NT.Parent,
      "Root" => NT.Root,
      "Current" => NT.Current,
      "PropKey" => NT.PropKey,
      "RootNodes" => NT.RootNodes,
      "Header" => NT.Header,
      "TokenGroup" => NT.TokenGroup,
      "TokenValue" => NT.TokenValue,
      _ => NT.Null,
    };
  }
  public static void ErrorIfNull ([NotNull] object? current)
  {
    if (current is null)
    {
      Debug.Log(MsgClass.Error, "Node was null. Cannot Add Content.", "StandardRuleSets");
      throw new InvalidOperationException("Node was null. Cannot Add Content.");
    }
  }

  public static void DebugMessage (string message)
  {
#if DEBUG
    Debug.Log(MsgClass.GreenInfo, message, "NodeRuleHelper");
#endif
  }
}
