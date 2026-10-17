namespace Common.Nodes;

public class InvalidTokenException : InvalidOperationException
{
  public InvalidTokenException (string message) : base(message) { }
  public InvalidTokenException (string message, Exception innerException) : base(message, innerException) { }
  public InvalidTokenException () { }
}
