#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

namespace Common.Entities;
public class ParsingException : InvalidOperationException
{
  [DoesNotReturn]
  public static dynamic ThrowStackMismatch() => throw new("Stack mismatch. Attempted to pop from an empty stack.");
  [DoesNotReturn]
  public static dynamic ThrowNullData (string key) => throw new($"Attempted to write null data to key {key}.");
  [DoesNotReturn]
  public static dynamic ThrowValidationFailed (string message) => throw new($"Validation Failed. {message}");
  public ParsingException (string message) : base(message) { }
  public ParsingException (string message, Exception innerException) : base(message, innerException) { }
  public ParsingException () { }
}
