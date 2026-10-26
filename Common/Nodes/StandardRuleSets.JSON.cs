#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using static Common.Nodes.NodeRuleHelper;

using NT = Common.Nodes.NodeTarget;

namespace Common.Nodes;

public static partial class StandardRuleSets
{
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
        ErrorIfNull(token.Value);
        JSONObjectNode node = context.GenerateNode<JSONObjectNode>(NT.Current);
        context.AppendTokenTo(NT.Current, token);
        context.PushAndAddTo(node, NT.AnyRootOrParent);
        DebugMessage("Object Open Token Processed.");
      }
    };
    public static readonly NodeRule ObjectClose = new()
    {
      TokenName = "o_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        var node = context.PopTo(NT.Current);
        node.AppendToSuffix(token.Value);
        context.ClearNode(NT.Current);
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
}
