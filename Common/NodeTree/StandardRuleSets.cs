#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Common.Entities;
using static Common.NodeTree.NodeRuleHelper;

namespace Common.NodeTree;

public static class NodeRuleHelper
{
  extension (string className)
  {
    public object? CreateClass () =>
      Type.GetType(className)?.InvokeMember(SE, BFCI, null, null, null, CIIC);
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
  public static INode GetParent (NodeContext context) => context.NodeStack.Peek();

  public static void DebugMessage (string message)
  {
#if DEBUG
    Debug.Log(MsgClass.GreenInfo, message, "NodeRuleHelper");
#endif
  }

  public static void DoXMLOperation (NodeContext context, string action, string? assignTo, string? valueFrom, string? className)
  {
    switch(action)
    {
      case "GenerateNode":
        if (className is null || assignTo is null)
          throw new InvalidOperationException();
        if (className.CreateClass() is not INode node)
          throw new InvalidOperationException();
        switch(assignTo)
        {
          case "TokenValue" or "TokenGroup":
            throw new InvalidOperationException();
          case "Current":
            context.Current = node;
            break;
          case "Parent":
            if (context.Parent is not null)
            {
              _ = context.NodeStack.Pop();
              context.PushAndAddChild(node);
            }
            break;
        }
        break;
    }
  }
}

public static class StandardRuleSets
{
  private static class XML
  {
    public static readonly NodeRule ElementOpen = new() {
      TokenName = "element_open",
      Execute = static (context, _) =>
      {
        context.PushAndAddChild(context.GenerateCurrent<XMLElementNode>());
        DebugMessage("Element Open Tag Open Encountered. Assigning Current.");
      }
    };
    public static readonly NodeRule ElementClose = new() {
      TokenName = "element_close",
      Execute = static (context, _) =>
      {
        context.WarnIfCurrentHasData();
        context.Current = context.NodeStack.Pop();
        DebugMessage("Beginning Close Validation");
      }
    };
    public static readonly NodeRule ElementSingle = new()
    {
      TokenName = "element_single",
      Execute = static (context, token) =>
      {
        XMLElementNode node = context.GenerateCurrent<XMLElementNode>();
        node.Origin = token.Value;
        context.AddChildToParent(node);
        DebugMessage("Element Tag Single Encountered. Assigning Current.");
      }
    };
    public static readonly NodeRule ElementSingleEnd = new()
    {
      TokenName = "tag_close",
      Execute = static (context, _) =>
      {
        context.ClearCurrent();
        Debug.Log(MsgClass.GreenInfo, "Single Tag Closing. Adding as child to top of node stack. Setting Current to null.", "StandardRuleSets");
      }
    };
    public static readonly NodeRule ElementHeader = new()
    {
      TokenName = "element_header",
      Execute = static (context, _) =>
      {
        context.Header = context.GenerateCurrent<XMLElementNode>();
        DebugMessage("Element Tag Header Open Encountered. Assigning Header.");
      }
    };
    public static readonly NodeRule ElementHeaderEnd = new()
    {
      TokenName = "header_close",
      Execute = static (context, _) => context.ClearCurrent()
    };
    public static readonly NodeRule ElementCloseEnd = new()
    {
      TokenName = "close_tag_close",
      Execute = static (context, _) => context.ClearCurrent()
    };
    public static readonly NodeRule TagName = new()
    {
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
    public static readonly NodeRule AttributeName = new()
    {
      TokenName = "a_name",
      Execute = (context, token) =>
      {
        ErrorIfNull(context.Current);
        context.PropKey = token.Value;
        Debug.Log(MsgClass.GreenInfo, "Assigning PropKey.", "StandardRuleSets");
      }
    };
    public static readonly NodeRule AttributeValue = new()
    {
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
    public static readonly NodeRule Content = new()
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
    public static readonly NodeRule ElementOpenEnd = new()
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

    [SS("regex")] public const string RxElementClose = @"(?'element_close' < \s* /) \s* (?'tag_name' [:\w]+ ) \s* (?'close_tag_close' > )";
    [SS("regex")] public const string RxElementSingle = @"(?'element_single' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s*  (?'tag_close' / \s* > )";
    [SS("regex")] public const string RxElementHeader = @"(?'element_header' <\?) \s* (?'tag_name' xml ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'header_close' \?> )";
    [SS("regex")] public const string RxElementOpen = @"(?'element_open' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'open_tag_close' > )";
    [SS("regex")] public const string RxContent = @"(?<= >) (?'ws'\s*) (?'content'[^<]*?) (?'ws'\s*) (?=<)";
    [SS("regex")] public const string RxComment = @"(?'comment'<!-- ((?!--)[\s\S])* -->)";
  }

  private static class JSON
  {
    [SS("regex")]
    public const string RxAfterKey = @"(?<=[:=]\s*)";
    [SS("regex")] public const string RxKey = @"(?'qt'[""']) (?'key_name'\w+) \k<qt> (?=\s*[:=])";
    [SS("regex")] public const string RxString = @"(?<=[:=]\s*) (?'qt'[""']) (?'value'([^\\""]|\\.)*) \k<qt>";
    [SS("regex")] public const string RxNumber = @"(?<=[:=]\s*) (?'value'[0-9.xXa-fA-F-]+ )";
    [SS("regex")] public const string RxBool = @"(?<=[:=]\s*) (?'value'true|false)";
    [SS("regex")] public const string RxNull = @"(?<=[:=]\s*) (?'value'null)";
    [SS("regex")] public const string RxArray = @"(?'a_open'\[)|(?'a_close'\])";
    [SS("regex")] public const string RxObject = @"(?'o_open'\{)|(?'o_close'\})";
    [SS("regex")] public const string RxOps = "(?'op'[,:=])";
    [SS("regex")] public const string RxComment = @"(?'comment'\/\/.*|\/\* [\s\S]*? \*\/)";
    [SS("regex")] public const string RxWhitespace = @"(?'ws'\s+)";
    [SS("regex")] public const string RxErrorTrailingComma = @",\s*[\]}]";
    [SS("regex")] public const string RxErrorMissingComma = @"[}\]] \s* [\{[]";
    [SS("regex")] public const string RxErrorInvalidEscape = @"\\[^0nr\\""']";

    public static readonly NodeRule ElementOpen = new()
    {
      TokenName = "array_open",
      Execute = static (context, token) =>
      {
        JSONArrayNode node = context.GenerateCurrent<JSONArrayNode>();
        node.Origin = token.Value;
        context.PushAndAddChild(node);
        DebugMessage("Array Open Token Processed.");
      }
    };
  }

  public static TokenRuleSet JSONTokenRuleSet { get; } = new()
  {
    RegexOptions = ROCI | ROEC | ROIPW | ROML,
    TokenSequences =
    [
      JSON.RxKey,
      JSON.RxString,
      JSON.RxNumber,
      JSON.RxBool,
      JSON.RxNull,
      JSON.RxComment,
      JSON.RxObject,
      JSON.RxArray,
      JSON.RxWhitespace,
      JSON.RxOps
    ]
  };

  public static NodeRuleSet JSONRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = true },
    Rules = {
      XML.ElementOpen,
      XML.ElementOpenEnd,
      XML.ElementClose,
      XML.ElementCloseEnd,
      XML.ElementSingle,
      XML.ElementSingleEnd,
      XML.ElementHeader,
      XML.ElementHeaderEnd,
      XML.AttributeName,
      XML.AttributeValue,
      XML.Content,
      NodeRule.IgnoreGroup("comment"),
      NodeRule.IgnoreGroup("a_qt"),
      NodeRule.IgnoreGroup("a_eq"),
    },
  };

  public static TokenRuleSet XMLTokenRuleSet { get; } = new()
  {
    RegexOptions = ROCI | ROEC | ROIPW | ROML,
    TokenSequences =
    [
      XML.RxComment,
      XML.RxElementHeader,
      XML.RxElementSingle,
      XML.RxElementClose,
      XML.RxElementOpen,
      XML.RxContent
    ]
  };
  public static NodeRuleSet XMLRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = true },
    Rules = {
      XML.ElementOpen,
      XML.ElementOpenEnd,
      XML.ElementClose,
      XML.ElementCloseEnd,
      XML.ElementSingle,
      XML.ElementSingleEnd,
      XML.ElementHeader,
      XML.ElementHeaderEnd,
      XML.TagName,
      XML.AttributeName,
      XML.AttributeValue,
      XML.Content,
      NodeRule.IgnoreGroup("comment"),
      NodeRule.IgnoreGroup("a_qt"),
      NodeRule.IgnoreGroup("a_eq"),
    },
  };
}
