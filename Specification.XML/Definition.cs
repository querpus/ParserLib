#pragma warning disable RE0001 // Invalid regex pattern
#pragma warning disable CA1720 // Identifier contains type name

using Parser;
using Parser.Ops;
using Parser.Ops.Text;
using Parser.Tokens;

using static Parser.DefinitionStaticFunctions;
using static Parser.Tokens.TokenRuleType;

namespace Specification.XML;

/// <summary>The XML definition object.</summary>
[DefinitionExport]
public static class Definition
{
  //// <summary>
  //// <para>XML Regex for tokens</para>
  //// Old: <see href="https://regex101.com/r/PTKqnJ/3"/><br/>
  //// New: <see href="https://regex101.com/r/jcPotD/4"/>
  //// </summary>
  //public static RxSCollection Regex => [
  //  Nm("m_element", $@"<\s*(?'m_endtag'\/)?\s*{TagName}{Gp(WS + Attribute).Any}\s*(?'m_noinsidetag'\/)?\s*>"),
  //  Nm("m_header", $@"<\?\s*{TagName}{Gp(WS + Attribute).Any}\s*\?>"),
  //  Nm("m_ws", $"(?<=>){WS}(?=<)"),
  //  Nm("m_comment", "<!--(-(?!-)|[^-])*?-->"),
  //  Nm("m_content", "[^<]+")
  //];

  //// <summary>The attribute regular expression.</summary>
  //private static readonly RxS
  //  Attribute = Rx(@"(?'m_prop_key_1'\w+)\s*=\s*""(?'m_prop_value_1'.*?)"""),
  //  TagName = Nm("m_prop_tagname", "[A-Za-z][a-zA-Z0-9]+"),
  //  WS = RX.WS;

  /// <summary>The XML specification.</summary>
  [DefinitionExport]
  public static Spec Spec => new()
  {
    FileInferences = [
      IfN(ExtIs, "xml"),
      IfN(ExtIs, "xsd"),
      IfN(ExtIs, "cd"),
      IfN(ExtIs, "csproj"),
      IfN(HeadSt, "<?xm")
    ],
    Name = "xml",
    RxOpt = ROML | ROEC | ROIPW,
    IsTextFile = true,
    SC = SCO,
    TokenType = typeof(string),
    TokenRules = [
      new(Competitive, "DString", @"""[^""><]*"""),
      new(Competitive, "SString", "'[^'><]*'"),
      new(Competitive, "Comment", @"<!--((?!--).)*-->"),
      new (TokenMatch, "Content", @"(?<= >)[^<]+(?=<)"),
      new (TokenComment, "None", @"(?<=\>)\s+(?=\<)"),
      .. TokenRule.MakeSingleCharRules("<>/?;&:=!-", TokenExact, new Collection<string>() { "Ao", "Ac", "Sl", "Qm", "Sc", "An", "Co", "Eq", "Em", "Hy" }),
      new (TokenMatch, "NamespaceAttr", @"\bxmlns\b"),
      new (TokenMatch, "AttrKey", @"\b\w+\b(?=\s*\=)"),
      new (TokenMatch, "Namespace", @"(?<= <\/?\s* )\b\w+\b(?=\:)"),
      new (TokenMatch, "ElementName", @"(?<= <\??\/?\s*(\w+\:)? )\b\w+\b(?=\s*[^\=<])"),
      new (ErrorMatch, "None", @"\<\?(?<error_pos>\w+)\b(?<!xml)"),           // No non-xml headers
      new (ErrorMatch, "None", @"\<\w+(?<error_pos>\s+)\w+\b\/?\>"),          // No spaces in element names
    ],
    GroupTokenRules = [
      new ("DocumentNamespace", "t:NamespaceAttr x:Co n:AttrKey x:Eq v:String"),
      new ("DocumentNamespace", "t:NamespaceAttr x:Eq v:String"),
      new ("AttributeWithNamespace", "t:Namespace x:Co n:AttrKey"),
      new ("Attribute", "n:(AttrKey|AttributeWithNamespace) x:Eq v:String"),
      new ("Header", "x:Ao x:Qm n:ElementName{xml} pa:Attribute x:Qm x:Ac"),
      new ("TagName", "t:Namespace x:Co n:ElementName"),
      new ("ElementEnd", "x:Ao x:Sl n:(ElementName|ElementWithNamespace) x:Ac"),
      new ("ElementSingle", "x:Ao n:(ElementName|ElementWithNamespace) pa:Attribute x:Sl x:Ac"),
      new ("ElementStart", "x:Ao n:(ElementName|ElementWithNamespace) pa:Attribute x:Ac"),
      new (Recursive, "ElementPair", "d:ElementStart va:ValidContent x:ElementEnd"),

    ],
    TokenCompatLookup = {
      ["String"] = ["DString", "SString"],
      ["Attribute"] = ["DocumentNamespace", "AttributeWithNamespace"],
      ["ValidContent"] = ["Content", "ElementSingleWithNamespace", "ElementSingle", "ElementPair"]
    },
    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens" },
      new DebugPrintKeyOperation { InputKey = "tokens" },
      new FilterTokenOperation { InputKey="tokens", OutputKey="tokens_filtered", StrType="Whitespace" },
      new DebugPrintKeyOperation { InputKey = "tokens_filtered" },
      new TokenAssembleOperation { InputKey = "tokens_filtered", OutputKey = "tokens_assembled" },
      new DebugPrintKeyOperation { InputKey = "tokens_assembled" },
    ],
  };
}
