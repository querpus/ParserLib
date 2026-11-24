namespace Parser.Exceptions;

/// <summary>Thrown when a cursor overruns its maximum size, or an attempt to access a non-existant array item occurs.</summary>
public class OperationBufferOverflowException : OperationException
{
  public OperationBufferOverflowException () : base("Buffer overflow. Attempted to access data past the end of the file.") { }
  public OperationBufferOverflowException (int position_attempted, int max_size) : base($"Buffer overflow. Attempted to access data at position {position_attempted}, past the end of the file at {max_size}.") { }
  public OperationBufferOverflowException (string? message, Exception? innerException) : base(message, innerException) { }
  public OperationBufferOverflowException (string message) : base(message) { }
}
