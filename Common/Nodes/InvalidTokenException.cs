namespace Common.Nodes;

public class InvalidTokenException : InvalidOperationException
{
  public InvalidTokenException (string message) : base(message) { }
  public InvalidTokenException (string message, Exception innerException) : base(message, innerException) { }
  public InvalidTokenException () : base("Invalid Token Encountered.") { }
}

public class InvalidNodeException : InvalidOperationException
{
  public InvalidNodeException (string message) : base(message) { }
  public InvalidNodeException (string message, Exception innerException) : base(message, innerException) { }
  public InvalidNodeException () : base("Invalid Node Encountered.") { }
}
