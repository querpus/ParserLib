namespace Common;

/// <summary>Defines the programs log level.</summary>
/// <remarks>This is how the program displays messages, not for the messages themselves.</remarks>
public enum LogClass
{
  /// <summary>Only show forced messages.</summary>
  None,
  /// <summary>Only show forced messages and error messages.</summary>
  Error,
  /// <summary>Show all errors and warnings.</summary>
  Warning,
  /// <summary>Show all status messages.</summary>
  Verbose,
  /// <summary>Show all messages, even debugging ones.</summary>
  Debug
}
