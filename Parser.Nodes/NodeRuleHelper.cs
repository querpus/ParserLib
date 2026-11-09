#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;

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
      "RootNodes" => NT.RootNodes,
      "Header" => NT.Header,
      "TokenGroup" => NT.TokenGroup,
      "TokenValue" => NT.TokenValue,
      _ => NT.None,
    };
  }
  public static void ErrorIfNull ([NotNull] object? current)
  {
    if (current is null)
    {
      Log(MsgClass.Error, "Node was null. Cannot Add Content.", "StandardRuleSets");
      throw new InvalidOperationException("Node was null. Cannot Add Content.");
    }
  }

  public static void DebugMessage (string message)
  {
#if DEBUG
    Log(MsgClass.GreenInfo, message, "NodeRuleHelper");
#endif
  }
}
