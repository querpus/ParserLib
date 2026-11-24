#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1720 // Identifier contains type name

namespace Specification.ZDoom.Lang.MapInfo;

public enum MapInfoTokenType
{
  None,
  AddDefaultMap,
  DefaultMap,
  Map,
  BlockKeyword,
  Bool,
  Char,
  Dec,
  Int,
  String,
  Class,
  LangRef,
  Value,
  Name,
  Include,
  GameInfo,
  Cluster,
  Skill,
  DamageType,
  Doomednums,
  Episode,
  Intermission,

  Op,

  PropertyName,
  Cast,
  Fader,
  GotoTitle,
  Image,
  Scroller,
  TextScreen,
  Wiper,
  Cutscene,
  Property,
}
