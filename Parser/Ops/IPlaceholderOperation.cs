namespace Parser.Ops;

/// <summary>For operations that require some work by the parser before they can be iterated through.</summary>
public interface IPlaceholderOperation
{
  /// <summary>Unpacks the operation into a flat structure.</summary>
  /// <param name="operations">The operation list reference.</param>
  /// <param name="index">The index of this operation in the sequence.</param>
  /// <param name="parser_ref">The parser or strategy reference.</param>
  /// <returns>The operation count after unpacking.</returns>
  int Unpack ([NotNull] Collection<IOperation> operations, int index, XParser parser_ref);
  /// <summary>Checks if the operation properly unpacked.</summary>
  /// <returns>Returns if the operation unpacked correctly, otherwise it will throw a <see  cref="OperationUnpackingException"/>.</returns>
  /// <exception cref="OperationUnpackingException">The operation did not unpack correctly.</exception>
  void CheckUnpacked ();
}
