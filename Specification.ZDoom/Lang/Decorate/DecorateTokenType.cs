#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1720 // Identifier contains type name
//#pragma warning disable IDE1006 // Naming Rule Violation

namespace Specification.ZDoom.Lang.Decorate;

//Decorate Prototype
//https://regex101.com/r/YtlFqj/1
//https://regex101.com/r/mTwORe/2

public enum DecorateTokenType
{
  Unknown,

  // Constructs
  States,
  Actor,
  Property,
  StateLabel,
  FrameDefinition,

  // Base Types
  Name,
  Keyword,
  Decimal,
  Int,
  String,
  Bo,
  Bc, // { }
  Po,
  Pc, // ( )
  Co,
  Cm, // : ,
  Pl,
  Mn, // + -
  StateName,
  FlagName,
}
