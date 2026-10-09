#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public static class StandardRuleSets
{
  private static void WarnIfCurrentHasData (NodeContext context)
  {
    if (context.Current is not null)
    {
      Debug.Log(MsgClass.Warning, "Current was not null before assignment.", "StandardRuleSets");
    }
  }
  private static void ErrorIfNull ([NotNull] object? current)
  {
    if (current is null)
    {
      Debug.Log(MsgClass.Error, "Node was null. Cannot Add Content.", "StandardRuleSets");
      throw new InvalidOperationException("Node was null. Cannot Add Content.");
    }
  }

  internal static NodeRule _element_open = new() {
    TokenName = "element_open",
    Execute = (context, token) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = new XMLElementNode() { Origin = token.Value };
      Debug.Log(MsgClass.GreenInfo, "Element Open Tag Open Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _element_close = new() {
    TokenName = "element_close",
    Execute = (context, _) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = context.NodeStack.Pop();
      Debug.Log(MsgClass.GreenInfo, "Beginning Close Validation", "StandardRuleSets");
    }
  };
  internal static NodeRule _element_single = new() {
    TokenName = "element_single",
    Execute = (context, token) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = new XMLElementNode() { Origin = token.Value };
      Debug.Log(MsgClass.GreenInfo, "Element Tag Single Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _tag_name = new() {
    TokenName = "tag_name",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);

      if (context.Current.Data["Name"] is not string prev)
      {
        context.Current.AddData("Name", token.Value);
        Debug.Log(MsgClass.GreenInfo, "Adding Name to Current.", "StandardRuleSets");
      }
      else if (prev.Equals(token.Value, SCO))
      {
        Debug.Log(MsgClass.GreenInfo, "Validation Passed", "StandardRuleSets");
      }
      else
      {
        Debug.Log(MsgClass.Warning, "Validation Failed", "StandardRuleSets");
      }
    }
  };
  internal static NodeRule _a_name = new() {
    TokenName = "a_name",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);
      context.PropKey = token.Value;
      Debug.Log(MsgClass.GreenInfo, "Assigning PropKey.", "StandardRuleSets");
    }
  };
  internal static NodeRule _a_value = new() {
    TokenName = "a_value",
    Execute = (context, token) =>
    {
      ErrorIfNull(context.Current);
      ErrorIfNull(context.PropKey);
      if (context.Current is XMLElementNode xen)
        xen.Attributes.Add(context.PropKey, new QString(token.Value));
      context.PropKey = null;
      Debug.Log(MsgClass.GreenInfo, "Adding attribute to current. Clearing PropKey.", "StandardRuleSets");
    }
  };
  internal static NodeRule _content = new()
  {
    TokenName = "content",
    Execute = (context, token) =>
    {
      if (context.IsRoot)
      {
        Debug.Log(MsgClass.GreenInfo, "Ignoring out of root content.", "StandardRuleSets");
      }
      else
      {
        var parent = context.NodeStack.Peek();
        parent.AddChild(new XMLContentNode() { Data = { ["Content"] = token.Value }, Origin = token.Value });
        Debug.Log(MsgClass.GreenInfo, "Adding content to node at top of node stack.", "StandardRuleSets");
      }
    }
  };
  internal static NodeRule _open_tag_close = new()
  {
    TokenName = "open_tag_close",
    Execute = (context, _) =>
    {
      ErrorIfNull(context.Current);
      if (context.Current is XMLElementNode xen)
      {
        if (context.IsRoot && context.Root is null)
        {
          context.Root = xen;
          context.NodeStack.Push(xen);
          context.Current = null;
          Debug.Log(MsgClass.Warning, "Element Open Tag Closed. Pushing Stack. Assigning Root.", "StandardRuleSets");
        }
        else if (context.IsRoot)
        {
          Debug.Log(MsgClass.Warning, "RootNode already defined.", "StandardRuleSets");
        }
        else
        {
          context.NodeStack.Peek().AddChild(context.Current);
          context.NodeStack.Push(xen);
          context.Current = null;
          Debug.Log(MsgClass.Warning, "RootNode already defined.", "StandardRuleSets");
        }
      }
    }
  };

  public static NodeRuleSet XMLRuleSet { get; } = new()
  {
    InitialSetup = () => new(),
    Rules = {
      _element_open,
      _element_close,
      _tag_name,
      _element_single,
      _a_name,
      _a_value,
      _open_tag_close,
      _content
    },
  };
}
