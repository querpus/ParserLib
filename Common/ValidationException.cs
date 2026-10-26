#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common;
public class ValidationException : Exception
{
  public ValidationException () { }
  public ValidationException (string? message) : base(message) { }
  public ValidationException (string? message, Exception?innerException) : base(message, innerException) { }
}
