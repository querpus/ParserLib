namespace Parser.Exceptions;

/// <summary>This is for quickly jumping back to the parser to quit the parse cycle.</summary>
public class QuitException : OperationException
{
  public QuitException ()
  {
  }

  public QuitException (string? message) : base(message)
  {
  }

  public QuitException (string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
