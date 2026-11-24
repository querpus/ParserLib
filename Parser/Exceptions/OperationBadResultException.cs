namespace Parser.Exceptions;

/// <summary>A catch-all exception that simply means the operation's result was not what it should be.</summary>
public class OperationBadResultException : OperationException
{
  public OperationBadResultException () : base("Bad Operation Result") { }
  public OperationBadResultException (string? message) : base(message) { }
  public OperationBadResultException (string? message, Exception? innerException) : base(message, innerException) { }
}
