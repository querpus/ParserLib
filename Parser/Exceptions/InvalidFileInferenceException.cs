namespace Parser.Exceptions;

/// <summary>Thrown when there is a problem parsing the file inferences.</summary>
public class InvalidFileInferenceException : OperationException
{
  public InvalidFileInferenceException () : base("Invalid inference declaration in spec definition.") { }
  public InvalidFileInferenceException (string? message) : base(message) { }
  public InvalidFileInferenceException (string? message, Exception? innerException) : base(message, innerException) { }
}
