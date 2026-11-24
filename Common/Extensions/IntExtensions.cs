//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.Extensions;

/// <summary>Integer extensions for bitwise operations.</summary>
public static class IntExtensions
{
  /// <summary>Integer Extensions</summary>
  /// <param name="value">The integer value to modify.</param>
  extension(int value)
  {
    /// <summary>Throws an exception if the value is below 0.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void ThrowIfNegative (string message)
    {
      if (value < 0)
        throw new InvalidValueException(nameof(value));
    }
    /// <summary>Checks if a specific flag is set in the integer value.</summary>
    /// <param name="flag">The flag to look for.</param>
    /// <returns>Returns <see langword="true"/> if the flag is set, <see langword="false"/> otherwise.</returns>
    public bool HasFlag (IConvertible flag) => (value & (int) flag) == (int) flag;
    /// <summary>Sets a specific flag in the integer value.</summary>
    /// <typeparam name="T">The type of integer value to return.</typeparam>
    /// <param name="flag">The flag to set.</param>
    /// <returns>The resulting value.</returns>
    public T SetFlag<T> (IConvertible flag) where T : IConvertible => (T) (IConvertible) (value | (int) flag);
    /// <summary>Masks a specific flag in the integer value.</summary>
    /// <typeparam name="T">The type of integer value to return.</typeparam>
    /// <param name="mask">The flag to mask.</param>
    /// <returns>The masked value.</returns>
    public T MaskFlag<T> (IConvertible mask) where T : IConvertible => (T) (IConvertible) (value & (int) mask);
    /// <summary>Toggles a specific flag in the integer value.</summary>
    /// <typeparam name="T">The type of integer value to return.</typeparam>
    /// <param name="bit">The flag to toggle.</param>
    /// <returns></returns>
    public T Toggle<T> (IConvertible bit) where T : IConvertible =>
      (T) (IConvertible) (
        HasFlag(value, bit) ?
        MaskFlag<T>(value, bit) :
        SetFlag<T>(value, bit)
      );
  }
}
