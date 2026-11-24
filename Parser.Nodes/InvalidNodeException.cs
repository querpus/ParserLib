namespace Parser.Nodes;

public class InvalidNodeException : InvalidOperationException
{
  public InvalidNodeException (string message) : base(message) { }
  public InvalidNodeException (string message, Exception innerException) : base(message, innerException) { }
  public InvalidNodeException () : base("Invalid Node Encountered.") { }
}
