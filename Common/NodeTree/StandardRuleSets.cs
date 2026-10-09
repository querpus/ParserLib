#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Common.Entities;
using static Common.NodeTree.NodeRuleHelper;

namespace Common.NodeTree;

public static class NodeRuleHelper
{
  public static void WarnIfCurrentHasData (NodeContext context)
  {
    if (context.Current is not null)
    {
      Debug.Log(MsgClass.Warning, "Current was not null before assignment.", "StandardRuleSets");
    }
  }
  public static void DebugIgnoredGroup (string group) =>
    Debug.Log(MsgClass.GreenInfo, $"Group {group} ignored.", "StandardRuleSets");
  public static void ErrorIfNull ([NotNull] object? current)
  {
    if (current is null)
    {
      Debug.Log(MsgClass.Error, "Node was null. Cannot Add Content.", "StandardRuleSets");
      throw new InvalidOperationException("Node was null. Cannot Add Content.");
    }
  }
  public static void ClearCurrent (NodeContext context) => context.Current = null;
  public static void CreateNewCurrent<TNode> (NodeContext context, EToken token) where TNode : INode, new()
  {
    WarnIfCurrentHasData(context);
    context.Current = new TNode { Origin = token.Value };
  }
  public static INode GetParent (NodeContext context) => context.NodeStack.Peek();

  public static void DebugMessage (string message)
  {
#if DEBUG
    Debug.Log(MsgClass.GreenInfo, message, "NodeRuleHelper");
#endif
  }
}

public static class StandardRuleSets
{
  internal static NodeRule _element_open = new() {
    TokenName = "element_open",
    Execute = static (context, token) =>
    {
      CreateNewCurrent<XMLElementNode>(context, token);
      DebugMessage("Element Open Tag Open Encountered. Assigning Current.");
    }
  };
  internal static NodeRule _element_close = new() {
    TokenName = "element_close",
    Execute = static (context, _) =>
    {
      WarnIfCurrentHasData(context);
      context.Current = context.NodeStack.Pop();
      DebugMessage("Beginning Close Validation");
    }
  };
  internal static NodeRule _element_single = new() {
    TokenName = "element_single",
    Execute = static (context, token) =>
    {
      CreateNewCurrent<XMLElementNode>(context, token);
      Debug.Log(MsgClass.GreenInfo, "Element Tag Single Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _element_header = new()
  {
    TokenName = "element_header",
    Execute = static (context, token) =>
    {
      CreateNewCurrent<XMLElementNode>(context, token);
      context.Header = context.Current;
      Debug.Log(MsgClass.GreenInfo, "Element Tag Header Encountered. Assigning Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _header_close = new()
  {
    TokenName = "header_close",
    Execute = static (context, _) =>
    {
      ClearCurrent(context);
      Debug.Log(MsgClass.GreenInfo, "Element Tag Header Close Encountered. Current is now null.", "StandardRuleSets");
    }
  };
  internal static NodeRule _tag_close = new()
  {
    TokenName = "tag_close",
    Execute = static (context, _) =>
    {
      ErrorIfNull(context.Current);
      GetParent(context).AddChild(context.Current);
      ClearCurrent(context);
      Debug.Log(MsgClass.GreenInfo, "Single Tag Closing. Adding as child to top of node stack. Setting Current to null.", "StandardRuleSets");
    }
  };
  internal static NodeRule _close_tag_close = new()
  {
    TokenName = "close_tag_close",
    Execute = static (context, _) =>
    {
      ErrorIfNull(context.Current);
      ClearCurrent(context);
      Debug.Log(MsgClass.GreenInfo, "Closing Tag Closing. Clearing Current.", "StandardRuleSets");
    }
  };
  internal static NodeRule _tag_name = new() {
    TokenName = "tag_name",
    Execute = static (context, token) =>
    {
      ErrorIfNull(context.Current);

      if (!context.Current.Data.TryGetValue("Name", out object? name_obj))
      {
        context.Current.AddData("Name", token.Value);
        DebugMessage("Adding Name to Current.");
      }
      else if (name_obj is string prev && prev.Equals(token.Value, SCO))
      {
        DebugMessage("Validation Passed");
      }
      else
      {
        ParsingException.ThrowValidationFailed($"XML Closing Tag Mismatched: <{name_obj as string}></{token.Value}>");
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
      if (token.Value.IsWhitespace)
      {
        Debug.Log(MsgClass.BlueInfo, "Ignoring whitespace.", "StandardRuleSets");
      }
      else if (context.IsRoot)
      {
        Debug.Log(MsgClass.Warning, "Ignoring out of root content.", "StandardRuleSets");
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
          Debug.Log(MsgClass.GreenInfo, "Element Open Tag Closed. Pushing Stack. Assigning Root.", "StandardRuleSets");
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
          Debug.Log(MsgClass.GreenInfo, "Element Open Tag CLosed. Pushing Stack.", "StandardRuleSets");
        }
      }
    }
  };

  public static NodeRuleSet XMLRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = true },
    Rules = {
      _element_open,
      _element_close,
      _tag_name,
      _element_single,
      _a_name,
      _a_value,
      _open_tag_close,
      _content,
      _element_header,
      _header_close,
      NodeRule.IgnoreGroup("a_qt"),
      NodeRule.IgnoreGroup("a_eq"),
      _tag_close,
      _close_tag_close
    },
  };
}
