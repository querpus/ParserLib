namespace Specification.IPL;

/// <summary>Represents a command to compare.</summary>
public struct IPLCommandTemplate : IEquatable<IPLCommandTemplate>, ISerializable
{
  /// <summary>The command type.</summary>
  public required string CmdType { get; set; }
  /// <summary>Is the command escaped?</summary>
  public bool IsEscaped { get; set; }
  /// <summary>Is the command shifted?</summary>
  public bool IsShifted { get; set; }
  /// <summary>Checks equality against another object.</summary>
  /// <param name="other">The other object</param>
  /// <returns><see langword="true"/> if the objects are the same command, <see langword="false"/> otherwise.</returns>
  public readonly bool Equals (IPLCommandTemplate other) =>
    CmdType == other.CmdType &&
    IsShifted == other.IsShifted &&
    IsEscaped == other.IsEscaped;

  /// <inheritdoc/>
  public override readonly bool Equals (object? obj) => Equals(obj is null ? Empty : (IPLCommandTemplate) obj);
  /// <summary>Gets the basic command formatted as readable text.</summary>
  /// <returns>The basic command formatted as readable text.</returns>
  public override readonly string ToString ()
  {
    string escape = IsEscaped ? "<ESC>" : SE;
    string shift = IsShifted ? "<SI>" : SE;
    string cmd = CmdType;
    return escape + shift + cmd;
  }
  /// <inheritdoc/>
  public override readonly int GetHashCode () => ToString().GetHashCode(SCO);
  /// <inheritdoc/>
  public readonly string Serialize () => ToString();

  /// <summary>An empty or null command.</summary>
  public static IPLCommandTemplate Empty { get; } = new() { CmdType = SE };
  /// <summary>Checks equality of 2 command objects.</summary>
  /// <param name="left">The lefthand object.</param>
  /// <param name="right">The righthand object.</param>
  /// <returns><see langword="true"/> if the objects are the same command, <see langword="false"/> otherwise.</returns>
  public static bool operator == (IPLCommandTemplate left, IPLCommandTemplate right) => left.Equals(right);
  /// <summary>Checks the inequality of 2 command objects.</summary>
  /// <param name="left">The lefthand object.</param>
  /// <param name="right">The righthand object.</param>
  /// <returns><see langword="true"/> if the objects are not the same command, <see langword="false"/> otherwise.</returns>
  public static bool operator != (IPLCommandTemplate left, IPLCommandTemplate right) => !(left == right);
}
