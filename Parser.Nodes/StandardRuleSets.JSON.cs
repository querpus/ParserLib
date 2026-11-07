#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using static Parser.Nodes.NodeRuleHelper;

using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;

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
    [SS("regex")] public static readonly string RxError = GenerateRegexForErrorChecking(ErrorList);

    public static readonly NodeRule ObjectOpen = new()
    {
      TokenName = "o_open",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);

        JSONObjectNode node = context.GenerateNode<JSONObjectNode>(NT.None);
        node.AppendToPrefix(token);

        if (context.Current is JSONArrayNode obj)
        {
          context.PushAndAddTo(node, NT.AnyRootOrParent);
        }
        else if (context.Current is JSONPropertyNode prop)
        {
          prop.Value = node;
          context.NodeStack.Push(node);
        }

        DebugMessage("Object Open Token Processed.");
      }
    };
    public static readonly NodeRule ArrayOpen = new()
    {
      TokenName = "a_open",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);

        JSONArrayNode node = context.GenerateNode<JSONArrayNode>(NT.None);
        node.AppendToPrefix(token);

        if (context.Current is JSONArrayNode obj)
        {
          context.PushAndAddTo(node, NT.AnyRootOrParent);
        }
        else if (context.Current is JSONPropertyNode prop)
        {
          prop.Value = node;
          context.NodeStack.Push(node);
        }

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
    public static readonly NodeRule ArrayClose = new()
    {
      TokenName = "a_close",
      Execute = static (context, token) =>
      {
        ErrorIfNull(token.Value);
        var node = context.PopTo(NT.Current);
        node.AppendToSuffix(token.Value);
        context.ClearNode(NT.Current);
        DebugMessage("Array Close Token Processed.");
      }
    };

    public static readonly NodeRule PropertyName = new()
    {
      TokenName = "key_name",
      Execute = static (context, token) =>
      {
        if (token.Value.IsEmpty)
        {
          throw new InvalidTokenException("Key cannot be null or empty.");
        }
        JSONPropertyNode node = context.GenerateNode<JSONPropertyNode>(NT.Current);
        node.Key = token.Value;
        node.AppendToPrefix(token.Value);
        context.AddNodeTo(node, NT.AnyRootOrParent);
        Log(MsgClass.GreenInfo, $"Property created with key {node.Key}", "StandardRuleSets");
      }
    };

    public static readonly NodeRule StringValue = new()
    {
      TokenName = "q_value",
      Execute = static (context, token) =>
      {
        JSONContentNode node = context.GenerateNode<JSONContentNode>(NT.None);
        node.Content = new QString(token.Value);
        if (context.Current is JSONPropertyNode prop)
        {
          prop.Value = node;
        }
        else if (context.Current is JSONArrayNode array)
        {
          array.Children.Add(node);
        }
        else
        {
          throw new InvalidTokenException("Attempted to assign string value to unknown type.");
        }
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
      JSON.RxOps,
      // Common error finder
      JSON.RxError,
    ]
  };
  public static NodeRuleSet JSONRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = true },
    Rules = {
      JSON.ObjectOpen,
      JSON.ObjectClose,
      JSON.ArrayOpen,
      JSON.ArrayClose,
      JSON.PropertyName,
      JSON.StringValue,
      NodeRule.KeepAndIgnore("op", appendToPrefix: true),
      NodeRule.KeepAndIgnore("ws"),
      NodeRule.KeepAndIgnore("comment"),
      NodeRule.Discard("qt"),
      NodeRule.Discard("str_value"),
      // Also look for errors
      NodeRule.SyntaxError("error")
    },
  };
}
