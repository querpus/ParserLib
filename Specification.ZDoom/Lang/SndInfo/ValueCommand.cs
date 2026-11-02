namespace Specification.ZDoom.Lang.SndInfo;

/// <summary>Stores a command and its parameters.</summary>
/// <param name="cmd">The command.</param>
/// <param name="values">The values.</param>
public class ValueCommand (string cmd, IEnumerable<string> values) : SndInfoCommand(cmd)
{
  /// <summary>The stored parameter values.</summary>
  public Collection<string> Values { get; init; } = [.. values];
}
