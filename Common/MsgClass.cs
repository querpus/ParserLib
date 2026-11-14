namespace Common;

/// <summary>Designates the color and required verbosity to display the message.</summary>
public enum MsgClass
{
  /// <summary>Do not use.</summary>
  [Obsolete("Must choose a class")]
  None,
  /// <summary>Displays when <see cref="LogClass.Debug"/> is set at program start.</summary>
  /// <remarks>Color is blue on black.</remarks>
  Debug,
  /// <summary>Always displays, for important program functionality.</summary>
  /// <remarks>Color is cyan on black.</remarks>
  Forced,
  /// <summary>Displays when <see cref="LogClass.Debug"/>, <see cref="LogClass.Error"/>, <see cref="LogClass.Warning"/>, or <see cref="LogClass.Verbose"/>  is set at program start.</summary>
  /// <remarks>Color is black on dark red.</remarks>
  Error,
  /// <summary>Displays when <see cref="LogClass.Error"/>, <see cref="LogClass.Warning"/> or <see cref="LogClass.Verbose"/>  is set at program start.</summary>
  /// <remarks>Color is yellow on black.</remarks>
  Warning,
  /// <summary>Displays when <see cref="LogClass.Debug"/>, <see cref="LogClass.Error"/>, <see cref="LogClass.Warning"/>, or <see cref="LogClass.Verbose"/>  is set at program start.</summary>
  /// <remarks>Color is black on red.</remarks>
  Critical,
  /// <summary>Displays when <see cref="LogClass.Debug"/> or <see cref="LogClass.Verbose"/> is set at program start.</summary>
  /// <remarks>Color is dark blue on black.</remarks>
  BlueInfo,
  /// <summary>Displays when <see cref="LogClass.Debug"/> or <see cref="LogClass.Verbose"/> is set at program start.</summary>
  /// <remarks>Color is light blue on black.</remarks>
  BlueValue,
  /// <summary>Displays when <see cref="LogClass.Debug"/> or <see cref="LogClass.Verbose"/> is set at program start.</summary>
  /// <remarks>Color is green on black.</remarks>
  GreenInfo,
  /// <summary>Not visible.</summary>
  /// <remarks>Color is black on black.</remarks>
  Hidden,
  /// <summary>Always displays, for user input requests.</summary>
  /// <remarks>Color is purple on black.</remarks>
  Prompt
}
