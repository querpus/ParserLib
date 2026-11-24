#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1720 // Identifier contains type name

namespace Specification.ZDoom.Lang.SndInfo;

public enum SndInfoTokenType
{
  Error = -1,
  None,
  Command,
  Bo,
  Bc,
  Eq,
  SoundFile,
  SoundLump,
  Integer,
  Decimal,
  String,
  /// <summary>PlayerSound keyword.</summary>
  PlayerSound,
  /// <summary>Alias keyword.</summary>
  Alias,
  /// <summary>Limit keyword.</summary>
  Limit,
  /// <summary>ArchivePath keyword.</summary>
  ArchivePath,
  /// <summary>PlayerSoundDup keyword.</summary>
  PlayerSoundDup,
  /// <summary>RollOff keyword.</summary>
  RollOff,

}
