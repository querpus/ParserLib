#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;

public static partial class StandardRuleSets
{
  private static class INI
  {
    public static NodeRule Section = new()
    {
      TokenName = "section",
      Execute = static (context) =>
      {
        INISectionNode node = context.GenerateNode<INISectionNode>(NT.Current | NT.Parent);

        context.SetTargetData(NT.Current, "Name", context.Token.Content[1..^1]);
        context.ClearNode(NT.Current);
        Log(MsgClass.GreenInfo, $"Section Defined: {context.Token.Content}", "StandardRuleSets");
      }
    };
    public static NodeRule Property = new()
    {
      TokenName = "property",
      Execute = static (context) =>
      {
        INIPropertyNode node = context.GenerateNode<INIPropertyNode>(NT.Current);
        context.AddNodeTo(node, NT.Parent);
        string[] halves = context.Token.Content.Split("=");
        node.Data["Key"] = halves[0];
        node.Data["Value"] = halves[1];
        Log(MsgClass.GreenInfo, $"Property Defined: {node.Key} / {node.Value}", "StandardRuleSets");
      }
    };
    [SS("regex")] private const string RxDQuotedString = @""" ([^\n\r\v\\""]|\\.)* """;
    [SS("regex")] private const string RxSQuotedString = @"' ([^\n\r\v\\]|\\.)* '";
    [SS("regex")] private const string RxWSNoNewLine = @"[^\S\n\v\r]*";
    [SS("regex")] public const string RxProperty = $@"(?'Key'[^;=\s][^;=\n\r\v]*(?<!\s)|{RxDQuotedString}|{RxSQuotedString}) {RxWSNoNewLine} (?'Eq'=) {RxWSNoNewLine} (?'Value'[^;\n]* (?<!\s) )";
    [SS("regex")] public const string RxComment = "(?'Comment';.*$)";
    [SS("regex")] public const string RxSection = @"(?'Ao'\[) (?'SectionName'.* ) (?'Ac'\])";
  }
  public static NodeRuleSet INIRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = false },
    Rules = {
      INI.Section,
      INI.Property
    },
  };
  public static TokenRuleSet INITokenRuleSet { get; } = new()
  {
    RegexOptions = ROCI | ROEC | ROIPW | ROML | ROIC,
    TokenSequences =
    [
      INI.RxComment,
      INI.RxProperty,
      INI.RxSection,
    ]
  };
}
