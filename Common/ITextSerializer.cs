namespace Common;
/// <summary>An interface specifying that this object can be represented by a <see langword="string"/>.</summary>
public interface ISerializable
{
  /// <summary>Creates a lossless <see cref="string"/> representation of the calling object.</summary>
  /// <returns>A lossless <see cref="string"/> representation of the calling object.</returns>
  string Serialize ();
  /// <summary>A <see cref="string"/> representation of the calling object, used for debugging or user readability.</summary>
  /// <returns>A summary or the object.</returns>
  string? ToString () => Serialize();
}
