#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using static Common.Nodes.NodeRuleHelper;

using NT = Common.Nodes.NodeTarget;

namespace Common.Nodes;

public static partial class StandardRuleSets
{
  private static class XML
  {
    #region Node Rules
    public static readonly NodeRule ElementOpen = new()
    {
      TokenName = "element_open",
      Execute = static (context, token) =>
      {
        XMLElementNode node = context.GenerateNode<XMLElementNode>(NT.Current);
        node.AppendToPrefix(token);
        context.PushAndAddTo(node, NT.AnyRootOrParent);
        DebugMessage($"Element Open Tag Open Encountered. Assigning Current. Pushing Current. Stack count:{context.NodeStack.Count}");
      }
    };
    public static readonly NodeRule ElementClose = new()
    {
      TokenName = "element_close",
      Execute = static (context, token) =>
      {
        context.WarnIfNotNull(NT.Current);
        _ = context.PopTo<XMLElementNode>(NT.Current);
        context.AppendTokenTo(NT.Current, token);
        DebugMessage($"Beginning Close Validation, Popping Current. Stack count:{context.NodeStack.Count}");
      }
    };
    public static readonly NodeRule ElementSingle = new()
    {
      TokenName = "element_single",
      Execute = static (context, token) =>
      {
        XMLElementNode node = context.GenerateNode<XMLElementNode>(NT.Current);
        node.AppendToPrefix(token);
        context.AddNodeTo(node, NT.Parent);
        DebugMessage("Element Tag Single Encountered. Assigning Current.");
      }
    };
    public static readonly NodeRule ElementSingleEnd = new() {
      TokenName = "tag_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        context.Current?.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    public static readonly NodeRule ElementHeader = new() {
      TokenName = "element_header",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        context.Header = context.GenerateNode<XMLElementNode>(NT.Current);
        context.Header.AppendToPrefix(token.Value);
        DebugMessage("Element Tag Header Open Encountered. Assigning Header.");
      }
    };
    public static readonly NodeRule ElementHeaderEnd = new()
    {
      TokenName = "header_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        context.Current?.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    public static readonly NodeRule ElementCloseEnd = new()
    {
      TokenName = "close_tag_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        context.Current?.AppendToSuffix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    public static readonly NodeRule TagName = new()
    {
      TokenName = "tag_name",
      Execute = static (context, token) =>
      {
        ErrorIfNull(context.Current);
        ErrorIfNull(token.Value);

        if (context.Current.Prefix?.Contains(token.Value) ?? false)
        {
          context.Current.AppendToSuffix(token.Value);
        }
        else
        {
          context.Current.AppendToPrefix(token.Value);
        }

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
          throw new ValidationException($"XML Closing Tag Mismatched: <{name_obj as string}></{token.Value}>");
        }
      }
    };
    public static readonly NodeRule AttributeName = new()
    {
      TokenName = "a_name",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        ErrorIfNull(context.Current);
        context.Current?.AppendToPrefix(" " + token.Value);
        context.PropKey = token.Value;
        Debug.Log(MsgClass.GreenInfo, "Assigning PropKey.", "StandardRuleSets");
      }
    };
    public static readonly NodeRule AttributeValue = new()
    {
      TokenName = "a_value",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        ErrorIfNull(context.Current);
        ErrorIfNull(context.PropKey);
        if (context.Current is XMLElementNode xen)
          xen.Attributes.Add(context.PropKey, new QString(token.Value));
        context.ClearNode(NT.PropKey);
        context.Current.AppendToPrefix(token.Value);
      }
    };
    public static readonly NodeRule Content = new()
    {
      TokenName = "content",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        if (token.Value.IsWhitespace)
        {
          Debug.Log(MsgClass.Warning, "Ignoring whitespace.", "StandardRuleSets");
        }
        else if (context.IsRoot)
        {
          Debug.Log(MsgClass.Warning, "Ignoring out of root content.", "StandardRuleSets");
        }
        else
        {
          XMLContentNode node = context.GenerateNode<XMLContentNode>(NT.Current);
          node.Content = token.Value;
          context.AddNodeTo(node, NT.Parent);
        }
      }
    };
    public static readonly NodeRule ElementOpenEnd = new()
    {
      TokenName = "open_tag_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        ErrorIfNull(context.Current);
        context.Current.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    #endregion
    #region Regex Sequences
    [SS("regex")] public const string RxElementClose = @"(?'element_close' < \s* /) \s* (?'tag_name' [:\w]+ ) \s* (?'close_tag_close' > )";
    [SS("regex")] public const string RxElementSingle = @"(?'element_single' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s*  (?'tag_close' / \s* > )";
    [SS("regex")] public const string RxElementHeader = @"(?'element_header' <\?) \s* (?'tag_name' xml ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'header_close' \?> )";
    [SS("regex")] public const string RxElementOpen = @"(?'element_open' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'open_tag_close' > )";
    [SS("regex")] public const string RxContent = @"(?<= >) (?'ws'\s*) ((?'content'[^<]+) (?'ws'\s*))? (?=<)";
    [SS("regex")] public const string RxComment = @"(?'comment'<!-- ((?!--)[\s\S])* -->)";
    #endregion Regex Sequences
  }
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
      NodeRule.KeepAndIgnore("comment"),
      NodeRule.KeepAndIgnore("ws"),
      NodeRule.KeepAndIgnore("a_qt", true),
      NodeRule.KeepAndIgnore("a_eq", true),
      NodeRule.KeepAndIgnore("a_end", true),
    },
  };
}
