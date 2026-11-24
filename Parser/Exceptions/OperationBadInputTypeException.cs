namespace Parser.Exceptions;

/// <summary>Thrown when the operation expects a different type of input than what it was given.</summary>
public class OperationBadInputTypeException : OperationException
{
  public OperationBadInputTypeException () : base("Bad input type passed to operation.") { }
  public OperationBadInputTypeException (string expected, string got) : base($"Bad input type passed to operation. Expected {expected}, got {got}.") { }
  public OperationBadInputTypeException (string? message, Exception? innerException) : base(message, innerException) { }
  public OperationBadInputTypeException (string message) : base(message) { }
}
