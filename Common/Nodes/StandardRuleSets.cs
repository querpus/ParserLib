#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

namespace Common.Nodes;

public static partial class StandardRuleSets
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

  public static NodeRuleSet INIRuleSet { get; } = new()
  {
    InitialSetup = () => new() { SingleRoot = false },
    Rules = {
      INI.Section,
      INI.Property
    },
  };
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
      JSON.PropertyName,
      JSON.StringValue,
      NodeRule.KeepAndIgnore("op", true),
      NodeRule.KeepAndIgnore("ws"),
      NodeRule.KeepAndIgnore("comment"),
      NodeRule.Discard("qt"),
      NodeRule.Discard("str_value"),
      // Also look for errors
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
      IPL.RxEmulationMode,
      IPL.RxPrintMode,

      IPL.RxPrintCommand,
      IPL.RxClearDataCommand,

      IPL.RxEscapedCommand_0,

      IPL.RxFormat,
      IPL.RxPage,

      IPL.RxLineCommand,

      IPL.RxTextDataProp,
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
      IPL.EscapeCommand,
      IPL.PropertyCommand,
      IPL.Comma,
      IPL.FormatCommand,
      IPL.PrintCommand,
      NodeRule.KeepAndIgnore("before"),
      NodeRule.KeepAndIgnore("after"),
      NodeRule.KeepAndIgnore("between"),
      NodeRule.ReplaceEmpty("end", appendToSuffix: true, if_empty_insert_text: ";"),
    ]
  };
}
