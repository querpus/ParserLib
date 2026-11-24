namespace Parser;

/// <summary>A static class to assist with creating operation sequences in <see cref="Spec"/> objects.</summary>
public static class Err
{
  /// <summary>Throws a buffer overflow exception.</summary>
  /// <param name="position">The desired position.</param>
  /// <param name="end_of_file">The largest possible position.</param>
  /// <returns>Does not return.</returns>
  /// <exception cref="OperationBufferOverflowException"/>
  [DoesNotReturn]
  public static dynamic ThrowBufferOver (int position, int end_of_file) => throw new OperationBufferOverflowException(position, end_of_file);
  /// <summary>Throws a bad input exception.</summary>
  /// <param name="expected">The expected input.</param>
  /// <param name="got">The input recieved.</param>
  /// <returns>Does not return.</returns>
  /// <exception cref="OperationBadInputTypeException"/>
  [DoesNotReturn]
  public static dynamic ThrowBadInput (string expected, string got) => throw new OperationBadInputTypeException(expected, got);
  /// <summary>Throws a no such var exception.</summary>
  /// <param name="key">The data key that did not exist.</param>
  /// <returns>Does not return.</returns>
  /// <exception cref="OperationNoSuchVarException"/>
  [DoesNotReturn]
  public static dynamic ThrowNoVar (string key) => throw new OperationNoSuchVarException(key);
  /// <summary>Throws a bad definition exception.</summary>
  /// <param name="msg">THe message to display.</param>
  /// <returns>Does not return.</returns>
  /// <exception cref="OperationBadDefinitionException"/>
  [DoesNotReturn]
  public static dynamic ThrowBadDef (string msg) => throw new OperationBadDefinitionException(msg);
  [DoesNotReturn]
  public static dynamic ThrowBadResult (string msg) => throw new OperationBadResultException(msg);
  [DoesNotReturn]
  public static dynamic ThrowNoSpec (string msg) => throw new SpecNotDefinedException(msg);
  /// <summary>Throws an operation unpacking exception.</summary>
  /// <param name="msg">The message to display.</param>
  /// <returns>Does not return.</returns>
  /// <exception cref="OperationUnpackingException"/>
  [DoesNotReturn]
  public static dynamic ThrowUnpacked (string msg) => throw new OperationUnpackingException(msg);
}
