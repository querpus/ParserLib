namespace Parser.Exceptions;

/// <summary>Thrown when the requested <see cref="Spec"/> does not exist in the <see cref="Library"/>.</summary>
public class SpecNotDefinedException : OperationException
{
  public SpecNotDefinedException () : base("Spec not defined. Cannot load spec.") { }
  public SpecNotDefinedException (string? message) : base(message) { }
  public SpecNotDefinedException (string? message, Exception? innerException) : base(message, innerException) { }
}
