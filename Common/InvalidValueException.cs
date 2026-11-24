//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common;

public class InvalidValueException : Exception
{
  public InvalidValueException () { }
  public InvalidValueException (string? v) : base($"Invalid value \'{v ?? "<null>"}\'. ") { }
  public InvalidValueException (string? bad_value, string good_value) : base($"Invalid value \'{bad_value ?? "<null>"}\', required {good_value}. ") { }
  public InvalidValueException (string? message, Exception? innerException) : base(message, innerException) { }
}
