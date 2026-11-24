namespace Specification.ZDoom.Lang.SndInfo;

/// <summary>An abstract sndinfo command.</summary>
/// <param name="cmd">The type of command.</param>
public abstract class SndInfoCommand (string cmd = "Error")
{
  /// <summary>The command type.</summary>
  public string Command { get; set; } = cmd;
}
