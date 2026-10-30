#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using static Common.Nodes.NodeRuleHelper;

using NT = Common.Nodes.NodeTarget;

namespace Common.Nodes;

public static partial class StandardRuleSets
{
  private static class IPL
  {
    public static NodeRule LineCommand = new()
    {
      TokenName = "line_cmd",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
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
          context.PushAndAddTo(node, NT.AnyRootOrParent);
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
        ErrorIfNull(token.Value);
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
        ErrorIfNull(token.Value);
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.AppendToPrefix(token.Value);
        node.CommandLetter = token.Value[0];

        if (context.PropKey is "<ESC>" or "\e")
        {
          node.Escaped = true;
          context.ClearNode(NT.PropKey);
        }

        context.AddNodeTo(node, NT.RootNodes);
      }
    };

    public static NodeRule FormatCommand = new()
    {
      TokenName = "format",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        if (context.Current is not IPLCommandNode cmd)
        {
          Debug.Log(MsgClass.Error, "Current is not a command node. Aborting Format Storage.", "StandardParsingSets");
          throw new InvalidOperationException("Current is not an IPL command node. Token Processing Halted.");
        }

        // If we erase the currently selected formula clear the context formula also.
        if (cmd.CommandLetter is 'E' && context.Data["Format"].Like(token.Value))
        {
          context.Data["Format"] = "0";
        }

        // If we set the format, then this command is a part of it too.
        if (cmd.CommandLetter is 'F')
        {
          cmd.Format = int.Parse(token.Value, CIIC);
          context.Data["Format"] = token.Value;
        }
        cmd.AddValue(token.Value);
        cmd.AppendToPrefix(token.Value);
      }
    };

    public static NodeRule PropertyCommand = new()
    {
      TokenName = "prop_cmd",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        IPLCommandNode node = context.GenerateNode<IPLCommandNode>(NT.Current);
        node.Prefix = token.Value;
        node.CommandLetter = token.Value[0];

        context.AddNodeTo(node, NT.Parent);
      }
    };

    public static NodeRule EscapeCommand = new()
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

    public static NodeRule PrintCommand = new()
    {
      TokenName = "etb",
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

    [SS("regex")] private const string RxEsc = @"(?'esc' <ESC> | \e )";
    [SS("regex")] private const string RxEnd = "(?'end' ;|$|(?=<))";

    [SS("regex")] public const string RxPrintMode = "(?'mode' R )" + RxEnd;
    [SS("regex")] public const string RxEscapedCommand_0 = RxEsc + "(?'cmd' [APc] )" + RxEnd;
    [SS("regex")] public const string RxEmulationMode = RxEsc + "(?'mode' c ) (?'value'0|1)?" + RxEnd;
    [SS("regex")] public const string RxAdvancedMode = RxEsc + "(?'mode' C ) (?'value'0|1)?" + RxEnd;
    [SS("regex")] public const string RxFormat = $@"(?'cmd' [EF] ) (?'format'\d{{1,3}}){RxEnd}";
    [SS("regex")] public const string RxPage = RxEsc + "(?'cmd' G ) (?'page'[0-9])";
    [SS("regex")] public const string RxClearDataCommand = @"(?'can'<CAN>|\u0018)";
    [SS("regex")] public const string RxPrintCommand = @"(?'etb'<ETB>|\u0017)";
    [SS("regex")] public const string RxLineCommand = @"(?'line_cmd' [BDHLMQSU] ) (?'index' \d+)" + RxEnd;
    [SS("regex")] public const string RxTextDataProp = "(?'prop_cmd' d ) (?'value' 3) (?'op' ,) (?'value' [^;<\\n,]*)" + RxEnd;
    [SS("regex")] public const string RxPropCommand_1 = @"(?'prop_cmd' [cdhlw] ) (?'value' -?\d+)" + RxEnd;
    [SS("regex")] public const string RxPropCommand_2 = @"(?'prop_cmd' [cdo] ) (?'value' -?\d+) (?'op' ,) (?'value' -?\d+)" + RxEnd;
    [SS("regex")] public const string RxPropCommand_7 =
      """
      (?# Property with 7 Values c17)
      (?'prop_cmd' c ) (?'value' -?\d+) (?'op' ,)
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
}
