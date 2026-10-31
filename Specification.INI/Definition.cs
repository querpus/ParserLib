using Common.Nodes;

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
    Name = "ini",
    FileInferences = [
      IfN(ExtIs, "ini"),
      IfN(ExtIs, "vnc"),
      IfN(ExtIs, "url"),
      IfN(ExtIs, "inf")],
    RxOpt = ROML | ROIPW | ROEC | ROIC,
    IsTextFile = true,
    SC = SCOIC,
    TokenType = typeof(ITT),

    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
    ],

    TokenRules = [
      new(TokenComment, ITT.None, ";.*"),
      new(Competitive, ITT.Value, @"(?<=(?: =))([^\\=\n;]|\\.)*(?=$|;)"),
      new(Competitive, ITT.Key, @"(?<=^\s*)([^\s\\=\n;]|\\.)*(?=\s*(=))"),
      new(TokenExact, ITT.Eq, "="),
      new(TokenExtract, ITT.Section, @"\[(?'keep'.*?)\]")],

    GroupTokenRules = [
      new(ITT.Property, "n:Key x:Eq v:Value"),
      new(ITT.SectionWProps, "n:Section pa:Property")],

    DefaultRuleSet = IgnoreCase,
    TokenRuleSet = StandardRuleSets.INITokenRuleSet,
  };
}
