//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RCS1263 // Invalid reference in a documentation comment

namespace Common.Extensions;

public static class IConvertibleExtensions
{
  /// <summary>IConvertible Extensions</summary>
  /// <param name="e">The enumeration to convert.</param>
  extension(IConvertible e)
  {
    /// <summary>Gets a numeric integer from any enumeration.</summary>
    /// <returns>The enumeration value.</returns>
    public int ToInt () => (int) e;
    /// <summary>Gets a 64 bit numeric integer from any enumeration.</summary>
    /// <returns>The enumeration value.</returns>
    public long ToLong () => (long) e;
    /// <summary>Checks if the enum contains any of the provided bits.</summary>
    /// <param name="check">The bits to check for.</param>
    /// <returns><see langword="true"/> if the enum contains any of the bits provided, <see langword="false"/> otherwise.</returns>
    public bool HasAnyFlag (IConvertible check) => ((int) e & (int) check) > 0;
    public bool IsWithin (IConvertible low, IConvertible high) =>
       (int) e <= (int) high && (int) e >= (int) low;
    public bool IsWithinLong (IConvertible low, IConvertible high) =>
       (long) e <= (long) high && (long) e >= (long) low;
  }
  /// <summary>More IConvertible Extensions</summary>
  /// <param name="i">The field to deflag.</param>
  extension(IConvertible i)
  {
    /// <summary>Removes all of the provided bits from an enum.</summary>
    /// <typeparam name="T">The type of enum to return.</typeparam>
    /// <param name="bit">The flags to remove.</param>
    /// <returns>An enum of the given type with the given flags removed.</returns>
    public T RemoveBit<T> (IConvertible bit) where T : IConvertible => (T) (IConvertible) ((int) i & ~(int) bit);
    public T RemoveBitLong<T> (IConvertible bit) where T : IConvertible => (T) (IConvertible) ((long) i & ~(long) bit);
  }

  /// <summary>More IConvertible Extensions</summary>
  /// <param name="value">The value to mask.</param>
  extension(IConvertible value)
  {
    public T Mask<T> (IConvertible mask) where T : IConvertible => (T) (IConvertible) ((int) value & (int) mask);
    public T MaskLong<T> (IConvertible mask) where T : IConvertible => (T) (IConvertible) ((long) value & (long) mask);
  }
}
