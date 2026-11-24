namespace Parser.Ops;

/// <summary>The interface for all operations in the parser.</summary>
public interface IOperation
{
  #region Operation Flags
  /// <summary>Specifies that the operation should not stop the parser upon failure.</summary>
  bool ContinueOnFail { get; set; }
  /// <summary>Specifies that the operation should be skipped.</summary>
  bool SkipOperation { get; set; }
  /// <summary>Specifies that the operation performs no action, and simply advances to the next operation.</summary>
  bool NoExecution { get; }
  #endregion
  /// <summary>Calls the operation with the parser provided.</summary>
  /// <param name="parser_ref">The parser to pull data from and to store data in.</param>
  /// <returns>An <see cref="OpStatus"/> that represents the result status.</returns>
  OpStatus DoOperation (XParser parser_ref);
  /// <summary>Applies these attributes to the operation.</summary>
  /// <param name="cont">Specifies that the operation should not stop the parser upon failure.</param>
  /// <param name="skip">Specifies that the operation should be skipped.</param>
  void ApplyProperties (bool cont, bool skip);
}
