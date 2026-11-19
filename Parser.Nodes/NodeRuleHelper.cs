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
  extension (IEnumerable<string> targets)
  {
    /// <summary>Converts a list of NodeTarget strings into a value.</summary>
    /// <returns>The <see cref="NT"/> value that the strings represent.</returns>
    public NT ToNodeTarget ()
    {
      NT result = NT.None;
      foreach (string target in targets)
      {
        result |= target.ToNodeTarget();
      }
      return result;
    }
  }
  extension (string target)
  {
    public NT ToNodeTarget () => target switch
    {
      "Parent" => NT.Parent,
      "Root" => NT.Root,
      "Current" => NT.Current,
      "Header" => NT.Header,
      "Temp" => NT.Temp,
      "Context" => NT.Context,
      "Data" => NT.Data,
      "Children" => NT.Children,
      "Properties" => NT.Properties,
      "TokenGroup" => NT.TokenGroup,
      "TokenValue" => NT.TokenValue,
      "Add" => NT.Add,
      "Remove" => NT.Remove,
      "Replace" => NT.Replace,
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
