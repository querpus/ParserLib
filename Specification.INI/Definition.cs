using Parser.Nodes;

using static Parser.DefinitionStaticFunctions;
using static Parser.Tokens.TokenRuleType;

namespace Specification.INI;

/// <summary>Defines the INI spec.</summary>
[DefinitionExport]
public static class Definition
{
  /// <summary>The INI Spec</summary>
  [DefinitionExport]
  public static Spec Spec => new()
  {
    Name = "ini_cs",
    FileInferences = [
      IfN(ExtIs, "ini"),
      IfN(ExtIs, "vnc"),
      IfN(ExtIs, "url"),
      IfN(ExtIs, "inf")],
    RxOpt = ROML | ROIPW | ROEC | ROIC,
    IsTextFile = true,
    SC = SCOIC,

    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
    ],

    TokenRules = [
      new(TokenComment, "None", ";.*"),
      new(Competitive, "Value", @"(?<=(?: =))([^\\=\n;]|\\.)*(?=$|;)"),
      new(Competitive, "Key", @"(?<=^\s*)([^\s\\=\n;]|\\.)*(?=\s*(=))"),
      new(TokenExact, "Eq", "="),
      new(TokenExtract, "Section", @"\[(?'keep'.*?)\]")],

    GroupTokenRules = [
      new("Property", "n:Key x:Eq v:Value"),
      new("SectionWProps", "n:Section pa:Property")],

    DefaultRuleSet = IgnoreCase,
    TokenRuleSet = StandardRuleSets.INITokenRuleSet,
    NodeRuleSet = StandardRuleSets.INIRuleSet
  };
}
