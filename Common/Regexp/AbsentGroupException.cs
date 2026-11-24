namespace Common.RegExp;

/// <summary>The exception thrown when a regex group is not found.</summary>
public class AbsentGroupException : KeyNotFoundException
{
  /// <summary>Creates a new <see cref="AbsentGroupException"/> with a named regex group as a reference.</summary>
  /// <param name="group">The named regex group to look for.</param>
  public AbsentGroupException (string group) : base($"Group '{group}' not found.") { }
  public AbsentGroupException (string message, Exception innerException) : base(message, innerException) { }
  public AbsentGroupException () { }
}
