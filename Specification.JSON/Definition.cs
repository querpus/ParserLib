#pragma warning disable RE0001 // Invalid regex pattern

using static Specification.JSON.JSONTokenType;

using RT = Parser.Tokens.TokenRuleType;

namespace Specification.JSON;

/// <summary>Defines a JSON Specification.</summary>
[DefinitionExport]
public static class Definition
{
  [DefinitionExport]
  public static Spec Spec => new()
  {
    Name = "json",
    FileInferences = [IfN(ExtIs, "json")],
    Operations = [
        //Operation.CreateCursor("json_cursor", 0),
        //ByteReadOperation.ReadRemainingBin("json_utf8", "json_cursor"),
        //TODO: Feed to JSON Reader
        //Operation.CopyKey("json_object", "result"),
        //TODO: Enhance Spec to analyse content.
        //TODO: Validate?
        new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" },
        new DebugToStringOperation { InputKey = "tokens" },
        new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
        new DebugToStringOperation { InputKey = "tokens_assembled" },
      ],
    RxOpt = ROML | ROIPW | ROEC | ROSL,
    IsTextFile = true,
    SC = SCO,
    GroupTokenRules = [
      new(RT.Recursive, Property, "n:Str x:Co v:Value"),
      new(RT.Recursive, JTT.Array, "x:Ao va:Value x:Ac"),
      new(RT.Recursive, JTT.Object, "x:Bo va:Property x:Bc"),
    ],
    TokenType = typeof(JTT),
    TokenCompatLookup = {
      [Value] = [Null, Bool, JTT.Array, Undef, Num, Str, JTT.Object],
    },
    TokenRules = [
      new(RT.Competitive, Str, $"\"{Gp(@"[^\\]|\\.").Any.Lazy}\""),
      new(RT.TokenMatch, Num, @"-?(\d+(\.\d*)?|\.\d+)"),
      .. TokenRule.MakeWordMatchRules(false, [
        ("true", Bool),
        ("false", Bool),
        ("null", Null),
        ("undefined", Undef)
      ]),
      .. TokenRule.MakeSingleCharRules("{}[]:,", RT.TokenExact, new Collection<JTT>() {Bo,Bc,Ao,Ac,Co,Cm}),
      new(RT.ErrorMatch, None, @"}(?=[^}]*\z)\s*(?<error_pos>\S[\s\S]*?)\s*\z"),
      new(RT.ErrorMatch, None, @"[]}](?=[^}\][{]*[\]\}])\s*(?<error_pos>\,)\s*[\}\]]"),
      ]
  };
}
