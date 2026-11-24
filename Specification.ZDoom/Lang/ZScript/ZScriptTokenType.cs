#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Rule Violation

namespace Specification.ZDoom.Lang.ZScript;

//Decorate Prototype
//https://regex101.com/r/YtlFqj/1

//ZScript Tokenizer
//https://regex101.com/r/dM72bX/1

public enum ZScriptTokenType
{
  None,
  Class,
  Include,
  Num,
  String,
  Name,
  State,
  Keyword,
  Comment,
  AddFlag,
  SubFlag,
  Property,
  FunctionCall,
  StateCmd,
  Value,
  Array,
  ClassRef,
  Bo,
  Bc,
  Po,
  Pc,
  Default,
  Sc,
  Cm,
  Co,
  Eq,
  Pl,
  Mn,
  FrameDef,
  FrameLump,
  GotoCmd,
  LoopCmd,
  BasicCmd,
  StateEntry,
  ActionSpecial,
  FunctionDef,
  MixinDef,
  States,
}
