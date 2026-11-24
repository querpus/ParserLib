namespace Parser.Exceptions;

/// <summary>Thrown when a formatting or structural issue with parsing an expression string occurs.</summary>
public class OperationConditionStringException : OperationException
{
  public OperationConditionStringException () : base("Condition String Contained Errors") { }
  public OperationConditionStringException (string? message) : base(message) { }
  public OperationConditionStringException (string? message, Exception? innerException) : base(message, innerException) { }
}
