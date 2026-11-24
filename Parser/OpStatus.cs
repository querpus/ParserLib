namespace Parser;

/// <summary>
/// The operation result status.
/// <para>Used to determine the result of an operation in the parser.</para>
/// </summary>
public enum OpStatus
{
  /// <summary>Not returned by any operation, but used to indicate that any operation code is accepted when checking status.</summary>
  Any = -2,
  /// <summary>Should never be returned by any operation, but is used to indicate that an error has occurred.</summary>
  Error = -1,
  /// <summary>The initial status of an operation, used to indicate that the operation has not yet started.</summary>
  AtStart = 0,
  /// <summary>Indicates that the operation has completed successfully.</summary>
  Pass = 1,
  /// <summary>Represents a state where an operation or process was skipped.</summary>
  Skipped = 2,
  /// <summary>Represents a state where an operation failed, but the parser should continue processing other operations.</summary>
  FailOverride = 4,
  /// <summary>The if condition passed.</summary>
  ConditionPass = 16,
  /// <summary>The if condition failed.</summary>
  ConditionFail = 32,
  /// <summary>The loop was exited.</summary>
  LoopBreak = 64,
  /// <summary>The loop advanced to the next iteration.</summary>
  /// <remarks>This is set by <see cref="ForCountOperation"/>, <see cref="ForEachOperation"/>, and <see cref="WhileOperation"/> loops.</remarks>
  LoopContinue = 128,
  /// <summary>A generic failure status that indicates an operation has failed.</summary>
  /// <remarks>Caused by a generic <see cref="OperationException"/>.</remarks>
  Fail = 256,
  /// <summary>Represents a failure condition where the input is null.</summary>
  FailBadInputNull = Fail * 2,
  /// <summary>Represents a failure condition where the operation definition is invalid or malformed.</summary>
  FailBadOpDefinition = Fail * 3,
  /// <summary>Represents a failure condition where the operation result is not valid or does not meet the expected criteria.</summary>
  FailBadOpResult = Fail * 4,
  /// <summary>Represents a failure condition where the operation is impossible to perform due to the current state or context.</summary>
  FailBadOpImpossible = Fail * 5,
  /// <summary>Represents a failure condition where the inference process has failed, such as when the parser cannot determine what type of specification to use.</summary>
  FailInference = Fail * 6,
  /// <summary>Represents a failure condition where the operation has exceeded the buffer size or capacity, leading to an overflow error.</summary>
  /// <remarks>This is specific to parsers that handle data in buffers, such as byte parsers.</remarks>
  FailBufferOverflow = Fail * 7,
  /// <summary>Represents a failure condition where the operation cannot proceed because no specification was provided or found.</summary>
  FailNoSpec = Fail * 8,
  /// <summary>Represents a failure condition where the operation cannot proceed because a required variable name was not found or is missing.</summary>
  FailNoSuchVarName = Fail * 9,
  /// <summary>Represents a failure condition where the input data type is not compatible with the expected type for the operation.</summary>
  FailBadInputType = Fail * 10,
  /// <summary>Represents a failure condition where the operation result is null, which is not acceptable for the operation's requirements.</summary>
  FailNullOpResult = Fail * 11,
  /// <summary>Represents a failure condition where the operation has failed due to a past failure that was not handled or resolved.</summary>
  PastFail = Fail * 12,
  /// <summary>Indicates that the operation sequence has been failed via an action in defined in the specification.</summary>
  DefinedFail = Fail * 13,
  /// <summary>Indicates that the parser was not passed any initial input.</summary>
  FailNoInput = Fail * 14,
  /// <summary>Indicates that the operation sequence has been completed successfully, and no further operations are needed.</summary>
  EndCommand = 65536,
}
