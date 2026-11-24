namespace Common.RegExp;

/// <summary>Exception thrown when a match group is empty but is required to contain characters.</summary>
public class EmptyGroupException : KeyNotFoundException
{
  public EmptyGroupException () { }
  public EmptyGroupException (string group) : base($"Group '{group}' is Empty.") { }
  public EmptyGroupException (string message, Exception innerException) : base(message, innerException) { }
}
