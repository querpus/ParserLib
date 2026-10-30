using Parser;
using Parser.Ops.Text;
using Parser.Tokens;

using static Parser.DefinitionStaticFunctions;

using RTT = Specification.REG.RegTokenType;

namespace Specification.REG;

public enum RegTokenType
{
  None,

  Head,
  KeyPart,
  AddVal,
  SubVal,
  AddDefVal,
  SubDefVal,
  DWordHex,
  QWordHex,
  HexPair,
  HexOne,
  AInt,
  Co,
  Eq,
  Sl,
  Cm,
  Bo,
  Bc,
  Po,
  Pc,
  At,
  Mn,
  TypeName,
  Key,
  AddKey,
  RemKey,
  AddProp,
  RemProp,
};

/// <summary>Defines the registry spec.</summary>
[DefinitionExport]
public static class Definition
{
  //private static readonly RxS
  //Head = Nm("header", Header.DefaultRx),
  //Remkey = Nm("remkey", @"\-"),
  //Remval = Nm("remval", @"\-"),
  //Sectchars = Rx(@"[ \w\\*-]"),
  //Sect = @"\[" + Remkey.Opt + Nm("section", Sectchars) + @"\]",
  //Def = Nm("default", @"\@").Opt,
  //Strprop = Nm("key", RX.CString),
  //Strval = Nm("value", RX.CString),
  //Hexval = Nm("type", Or(Nm("hex"), Nm("qword"), Nm("dword"))) + Gp(@"\(" + Nm("hsize", @"\d+") + @"\)").Opt + @"\:" + Nm("value", @"[0-9a-f, ]+"),
  //Prop = Nm("property", Or(Def, Strprop) + @"\=" + Or(Remval, Strval, Hexval));
  /// <summary>Defines the Registry spec.</summary>
  [DefinitionExport]
  public static Spec Spec => new()
  {
    Name = "reg",
    FileInferences = [IfN(ExtIs, "reg")],
    Operations = [
      new ReplaceOperation()
      {
        Nodes = [
          (@"\;.*$", ""), //Remove line comments
          (@"^\s+", ""), //Remove beginning ws
          (@"\s+$", ""), //Remove ending ws
          (@"\s*\=\s*", "="), //Remove ws around eq sign
          (@"\\" + RX.LnEnd, "]") //Remove escaped newlines
        ],
        InputKey = "text",
        OutputKey = "text"
      },
      new TokenizeOperation
      {
        InputKey = "text",
        OutputKey = "tokens",
        Tokenizer = "xparser"
      },
      new TokenAssembleOperation
      {
        InputKey = "tokens",
        OutputKey = "tokens_assembled"
      }
      ],
    TokenRules = [
      new(RT.TokenExact, RTT.Eq, "=")
    ],
    IsTextFile = true,
    RxOpt = ROIPW | ROEC | ROML,
    SC = SCOIC,
  };
}
