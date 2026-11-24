#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CA1720 // Identifier contains type name

namespace Specification.ZDoom.Lang.ModelDef;

public enum ModelDefTokenType
{
  None,

  //Keywords
  Model,
  Skin,
  Path,
  Scale,
  FrameIndex,

  //Flags
  PitchFromMomentum,
  InheritActorPitch,

  //Basic Structures
  Int,
  Decimal,
  String,
  Frame,
  FrameLetter,

  //Objects
  ModelFull,
  PropertyLine,
  FrameLine,

  Bo,
  Bc,
  Po,
  Pc,
  Cm,
  Sc,
  Eq,
  Co,
  Ao,
  Ac,
}
