#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using static Common.Nodes.NodeRuleHelper;

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

  public static void DebugIgnoredGroup (string group) => DebugMessage($"Group {group} ignored.");
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

  public static void DoXMLOperation (NodeContext context, CaptureToken token, string action, string? assignTo, string? valueFrom, string? className)
  {
    NT target = assignTo?.ToNodeTarget() ?? NT.Null;
    NT source = valueFrom?.ToNodeTarget() ?? NT.Null;

    switch(action)
    {
      case "GenerateNode" when className is not null:
        INode node = context.GenerateNode(target, className);
        node.AppendToPrefix(token.Value);
        break;
      case "AssignNode" when target is not NT.Null && source is not NT.Null:
        context.AssignNodeTo(context.GetTargetValue(source), target);
        break;
      case "AddToNode" when target is not NT.Null && source is not NT.Null:
        context.AddNodeTo(context.GetTargetValue(source), target);
        break;
      default:
        break;
    }
  }
}

public static class StandardRuleSets
{
  private static class Assembly
  {
    public static string GenerateRegexForMatchTokenizer ( IEnumerable<(string Name, string Regex)> list)
    {
      Collection<string> result = [];
      int index = 0;

      // Makes an index group to id the regex string used, and the name group to assign the token type.
      foreach (var item in list)
      {
        result.Add($"(?'_i_{index}' (?'_g_{item.Name}' {item.Regex}))");
        index++;
      }
      return result.TextJoin("|");
    }
    public static string GenerateRegexForErrorChecking (IEnumerable<(string ID, string Regex)> list)
    {
      string result = SE;
      int index = 0;

      foreach (var item in list)
      {
        result += (result.IsNotEmpty ? "|" : "(?'error'") + $"(?'_e_{index}' (?'_g_{item.ID}' {item.Regex}))";
        index++;
      }
      return result + ")";
    }
  }

  private static class XML
  {
    #region Node Rules
    public static readonly NodeRule ElementOpen = new()
    {
      TokenName = "element_open",
      Execute = static (context, token) =>
      {
        XMLElementNode node = context.GenerateNode<XMLElementNode>(NT.Current);
        node.Prefix = token.Value;
        context.PushAndAddTo(node);
        DebugMessage($"Element Open Tag Open Encountered. Assigning Current. Pushing Current. Stack count:{context.NodeStack.Count}");
      }
    };
    public static readonly NodeRule ElementClose = new()
    {
      TokenName = "element_close",
      Execute = static (context, _) =>
      {
        context.WarnIfNotNull(NT.Current);
        context.Current = context.NodeStack.Pop();
        DebugMessage($"Beginning Close Validation, Popping Current. Stack count:{context.NodeStack.Count}");
      }
    };
    public static readonly NodeRule ElementSingle = new()
    {
      TokenName = "element_single",
      Execute = static (context, token) =>
      {
        XMLElementNode node = context.GenerateNode<XMLElementNode>(NT.Current);
        node.Prefix = token.Value;
        context.AddNodeTo(node, NT.Parent);
        DebugMessage("Element Tag Single Encountered. Assigning Current.");
      }
    };
    public static readonly NodeRule ElementSingleEnd = new() {
      TokenName = "tag_close",
      Execute = static (context, token) =>
      {
        context.Current?.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    public static readonly NodeRule ElementHeader = new() {
      TokenName = "element_header",
      Execute = static (context, token) =>
      {
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
        context.Current?.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
      }
    };
    public static readonly NodeRule ElementCloseEnd = new()
    {
      TokenName = "close_tag_close",
      Execute = static (context, token) =>
      {
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
        ErrorIfNull(context.Current);
        ErrorIfNull(context.PropKey);
        if (context.Current is XMLElementNode xen)
          xen.Attributes.Add(context.PropKey, new QString(token.Value));
        context.PropKey = null;
        context.Current.AppendToPrefix(token.Value);
        Debug.Log(MsgClass.GreenInfo, "Adding attribute to current. Clearing PropKey.", "StandardRuleSets");
      }
    };
    public static readonly NodeRule Content = new()
    {
      TokenName = "content",
      Execute = static (context, token) =>
      {
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
          Debug.Log(MsgClass.GreenInfo, "Adding content to node at top of node stack.", "StandardRuleSets");
        }
      }
    };
    public static readonly NodeRule ElementOpenEnd = new()
    {
      TokenName = "open_tag_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(context.Current);
        context.Current.AppendToPrefix(token.Value);
        context.ClearNode(NT.Current);
        Debug.Log(MsgClass.GreenInfo, "Element Open Tag Closed. Clearing current", "StandardRuleSets");
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
  private static class IPL
  {
    public static NodeRule LineCommand = new()
    {
      TokenName = "line_cmd",
      Execute = static (context, token) =>
      {
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.Prefix = token.Value;
        node.CommandLetter = token.Value[0];

        if (context.HasParent)
        {
          _ = context.NodeStack.Pop();
          DebugMessage("Popping previous line command.");
        }

        if (context.IsRoot)
        {
          context.PushAndAddTo(node);
          DebugMessage("Pushing new line command.");
        }
        else
        {
          Debug.Log(MsgClass.Warning, "Warning: Context was not at root when pushing line command.", "StandardRuleSets");
        }
      }
    };

    public static NodeRule LineIndex = new()
    {
      TokenName = "index",
      Execute = static (context, token) =>
      {
        if (context.Current is IPLCommandNode icn)
        {
          icn.AddValue(token.Value);
          icn.Prefix += token.Value;
        }
        else
        {
          DebugMessage("Current was not an IPL node.");
        }
      }
    };

    public static NodeRule CmdEnd = new()
    {
      TokenName = "end",
      Execute = static (context, _) => context.ClearNode(NT.Current)
    };

    public static NodeRule BasicCommand = new()
    {
      TokenName = "cmd",
      Execute = static (context, token) =>
      {
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.Prefix = token.Value;
        node.CommandLetter = token.Value[0];

        if (context.PropKey is "<ESC>" or "\e")
        {
          node.Escaped = true;
          context.ClearNode(NT.PropKey);
        }

        context.AddNodeTo(node, NT.RootNodes);
      }
    };

    public static NodeRule PropertyCommand = new()
    {
      TokenName = "prop_cmd",
      Execute = static (context, token) =>
      {
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.Prefix = token.Value;
        node.CommandLetter = token.Value[0];

        context.AddNodeTo(node, NT.Parent);
      }
    };

    public static NodeRule Escape = new()
    {
      TokenName = "esc",
      Execute = static (context, token) =>
      {
        if (context.HasCurrent)
        {
          context.ClearNode(NT.Current);
        }
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.AppendToPrefix(token);
        node.Escaped = true;
      }
    };

    public static NodeRule Comma = new()
    {
      TokenName = "op",
      ExactText = ",",
      Execute = static (context, token) =>
      {
        if (context.HasCurrent)
          context.Current.AppendToPrefix(token);
        else
          throw new InvalidOperationException("No current node to operate on.");
      }
    };

    [SS("regex")] public const string RxSimpleCommand = "(?'cmd' [R] ) (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxEscapedCommand_0 = @"(?'esc' <ESC> | \e ) (?'cmd' [APc] ) (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxAdvancedMode = @"(?'esc' <ESC> | \e ) (?'mode' C ) (?'value'0|1)? (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxLineCommand = @"(?'line_cmd' [BHLMQSU] ) (?'index' \d+) (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxPropCommand_1 = @"(?'prop_cmd' [cdhlw] ) (?'value' -?\d+) (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxPropCommand_2 = @"(?'prop_cmd' [cdo] ) (?'value' -?\d+) (?'op' ,) (?'value' [^;<\n,]*) (?'end' ;|$|(?=<))";
    [SS("regex")] public const string RxPropCommand_7 =
      """
      (?# Property with 7 Values c17)
      (?'prop_cmd' [c] ) (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+) (?'op' ,)
      (?'value' -?\d+)(?'end' ;|$|(?=<))
      """;
    [SS("regex")] public const string RxBeforeStart = "(?'before' ^(?>((?!<STX>).)*)<STX>)";
    [SS("regex")] public const string RxBetweenMarkers = "(?'between'<ETX>(?>((?!<STX>).)*)<STX>)";
    [SS("regex")] public const string RxAfterEnd = "(?'after'<ETX>(?>((?!<STX>).)*))";
  }
  private static class JSON
  {
    [SS("regex")] public const string RxAfterKey = @"(?<=[:=]\s*)";
    [SS("regex")] public const string RxKey = @"(?'key' (?'qt'[""']) (?'key_name'\w+) \k<qt>) (?=\s*[:=])";
    [SS("regex")] public const string RxString = RxAfterKey + @"(?'q_value' (?'qt'[""']) (?'str_value'((?!\k<qt>).|\\.)*) \k<qt>)";
    [SS("regex")] public const string RxNumber = RxAfterKey + "(?'value'[0-9.xXa-fA-F-]+ )";
    [SS("regex")] public const string RxBool = RxAfterKey + "(?'bool_value'true|false)";
    [SS("regex")] public const string RxNull = @"(?<=[:=]\s*) (?'null_value'null)";
    [SS("regex")] public const string RxArray = @"(?'a_open'\[)|(?'a_close'\])";
    [SS("regex")] public const string RxObject = @"(?'o_open'\{)|(?'o_close'\})";
    [SS("regex")] public const string RxOps = "(?'op'[,:=])";
    [SS("regex")] public const string RxComment = @"(?'comment'\/\/.*|\/\* [\s\S]*? \*\/)";
    [SS("regex")] public const string RxWhitespace = @"(?'ws'\s+)";
    [SS("regex")] public const string RxErrorTrailingComma = @",\s*[\]}]";
    [SS("regex")] public const string RxErrorMissingComma = @"} \s* \{ | ] \s* \[";
    [SS("regex")] public const string RxErrorTypeChangeInArray = @"} \s* , \s*  \[ | ] \s* , \s* \{";
    [SS("regex")] public const string RxErrorTypeChangeInArrayAndNoComma = @"}  \s*  \[ | ]  \s* \{";
    [SS("regex")] public const string RxErrorInvalidEscape = @"\\[^0nr\\""']";
    public static readonly IEnumerable<(string, string)> ErrorList = [
      ("InvalidEscape", RxErrorInvalidEscape),
      ("MissingComma", RxErrorMissingComma),
      ("TrailingComma", RxErrorTrailingComma),
      ("ArrayTypeChange", RxErrorTypeChangeInArray),
      ("ArrayTypeChangeNoComma", RxErrorTypeChangeInArrayAndNoComma),
    ];
    [SS("regex")] public static readonly string RxError = Assembly.GenerateRegexForErrorChecking(ErrorList);

    public static readonly NodeRule ObjectOpen = new()
    {
      TokenName = "o_open",
      Execute = static (context, token) =>
      {
        JSONObjectNode node = context.GenerateNode<JSONObjectNode>(NT.Current);
        node.Prefix = token.Value;
        context.PushAndAddTo(node);
        DebugMessage("Object Open Token Processed.");
      }
    };
    public static readonly NodeRule ObjectClose = new()
    {
      TokenName = "o_close",
      Execute = static (context, token) =>
      {
        JSONObjectNode node = (JSONObjectNode) context.NodeStack.Pop();
        node.AppendToSuffix(token.Value);
        DebugMessage("Object Close Token Processed.");
      }
    };

    public static readonly NodeRule PropertyName = new()
    {
      TokenName = "key_name",
      Execute = static (context, token) =>
      {
        if (context.PropKey is not null)
          Debug.Log(MsgClass.Warning, "PropKey was not null when assigned.", "StandardRuleSets");

        context.PropKey = token.Value;
        context.Current?.AppendToPrefix("\n" + token.Value);
        Debug.Log(MsgClass.GreenInfo, "Assigning PropKey.", "StandardRuleSets");
      }
    };

    public static readonly NodeRule StringValue = new()
    {
      TokenName = "str_value",
      Execute = static (context, token) =>
      {
        if (context.PropKey is not null)
          Debug.Log(MsgClass.Warning, "PropKey was not null when assigned.", "StandardRuleSets");

        JSONContentNode node = context.GenerateNode<JSONContentNode>(NT.Current);
        node.AppendToPrefix(token);
        Debug.Log(MsgClass.GreenInfo, "Assigning PropKey.", "StandardRuleSets");
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
      JSON.ObjectOpen,
      JSON.ObjectClose,
      JSON.PropertyName,
      JSON.StringValue,
      NodeRule.KeepAndIgnore("op", true),
      NodeRule.KeepAndIgnore("ws"),
      NodeRule.KeepAndIgnore("comment"),
      NodeRule.Discard("qt"),
      NodeRule.Discard("str_value"),
      NodeRule.SyntaxError("error")
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
      NodeRule.KeepAndIgnore("comment"),
      NodeRule.KeepAndIgnore("ws"),
      NodeRule.KeepAndIgnore("a_qt", true),
      NodeRule.KeepAndIgnore("a_eq", true),
      NodeRule.KeepAndIgnore("a_end", true),
    },
  };
  public static TokenRuleSet IPLTokenRuleSet { get; } = new()
  {
    RegexOptions = ROIPW | ROML | ROEC,
    TokenSequences = [
      IPL.RxAdvancedMode,

      IPL.RxSimpleCommand,

      IPL.RxEscapedCommand_0,

      IPL.RxLineCommand,

      IPL.RxPropCommand_1,
      IPL.RxPropCommand_2,
      IPL.RxPropCommand_7,

      IPL.RxBeforeStart,
      IPL.RxBetweenMarkers,
      IPL.RxAfterEnd,
    ]
  };
  public static NodeRuleSet IPLRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = false },
    Rules = [
      IPL.LineCommand,
      IPL.BasicCommand,
      IPL.LineIndex,
      IPL.CmdEnd,
      IPL.Escape,
      IPL.PropertyCommand,
      IPL.Comma,
      NodeRule.KeepAndIgnore("before"),
      NodeRule.KeepAndIgnore("after"),
      NodeRule.KeepAndIgnore("between"),
      NodeRule.ReplaceEmpty("end", appendToSuffix: true, if_empty_insert_text: ";"),
    ]
  };
}
