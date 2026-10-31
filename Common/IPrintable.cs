//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common;

/// <summary>Defines a contract for objects that can output their content in a printable format with a specified indentation
/// level.</summary>
/// <remarks>Implementations should use the provided indentation level to format the output appropriately. This
/// interface is typically used to support custom printing or pretty-printing of objects for debugging or display
/// purposes.</remarks>
public interface IPrintable
{
  /// <summary>Sets the default indent size of each level.</summary>
  /// <param name="size">The number of spaces equal to one indent.</param>
  static void SetTabSize (int size) => TabSize = size;
  /// <summary>The default number of spaces to add to indent each level.</summary>
  static int TabSize { get; private set; } = 2;
  /// <summary>The indent at the start.</summary>
  static int StartingIndent { get; private set; }
  /// <summary>Prints this object to the console, preserving hierarchy.</summary>
  /// <param name="indent">The indent level.</param>
  /// <remarks>The argument <paramref name="indent"/> is the number of spaces.</remarks>
  void Print (int indent);
  /// <summary>Prints this object to the console at the base indent level.</summary>
  /// <remarks>The indent size is zero by default.</remarks>
  void Print () => Print(StartingIndent);
}
