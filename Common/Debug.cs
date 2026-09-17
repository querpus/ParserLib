using System.Runtime.CompilerServices;

using static Common.Chars;

namespace Common;

/// <summary>Static class containing debugging log functions and other useful tools.</summary>
public static class Debug
{
  #region Private Properties and Methods
  /// <summary>A pair of strings, that store the classname and the method name.</summary>
  /// <param name="ClassName">The classname.</param>
  /// <param name="Method">The method name.</param>
  public sealed record class StackLoc (string ClassName, string Method);
  public sealed record class StackRet (string KeyName, int StackPosition);
  public static Collection<StackLoc> CallStack { get; } = [];
  public static Collection<StackRet> Recovery { get; } = [];
  public static string ThisClass => CallStack?.Peek()?.ClassName ?? "ThisClass<CallStack is null>";
  public static string ThisMethod => CallStack?.Peek()?.Method ?? "ThisMethod<CallStack is null>";
  private static int LogDepth => CallStack.Count - 1;
  private static void DoLog (string msg, ConsoleColor? back = null, ConsoleColor? text = null, bool partial = false)
  {
    try
    {
      if (back is not null) Console.BackgroundColor = back.Value;
      if (text is not null) Console.ForegroundColor = text.Value;
      if (partial)
        Console.Write(msg);
      else
        Console.WriteLine(msg);
      if (back is not null || text is not null) Console.ResetColor();
    }
    catch (Exception)
    {
      throw;
    }
  }
  private static void DoLogHead (MsgClass cls, string classname, string method)
  {
    string format = $"{classname}.{method} : ";

    if (method.StartsWithAny(["[", "("], SCO))
      format = $"{classname}{method} : ";

    DoLog(format, GetBackColor(cls), GetTextColor(cls), true);
  }
  private static void DoLogHead (MsgClass cls, string caller) =>
    DoLog($"{caller} : ", GetBackColor(cls), GetTextColor(cls), true);
  private static ConsoleColor GetTextColor (MsgClass msg) => msg switch
  {
    MsgClass.Debug or MsgClass.BlueInfo => C_Blue,
    MsgClass.Forced => C_Cyan,
    MsgClass.Error or MsgClass.Critical or MsgClass.Hidden => C_Black,
    MsgClass.Warning => C_Yellow,
    MsgClass.GreenInfo => C_Green,
    MsgClass.Prompt => ConsoleColor.DarkMagenta,
    _ => C_White
  };
  private static ConsoleColor GetBackColor (MsgClass msg) => msg switch
  {
    MsgClass.Error => C_DarkRed,
    MsgClass.Critical => C_Red,
    _ => C_Black,
  };
  private static void PurgeStackTo (int depth)
  {
    while (CallStack.Count > depth + 1)
      CallStack.Drop();
    Log(MsgClass.Debug, "Purged back to here.", nameof(Debug));
  }
  private static void PurgeStackTo (string key)
  {
    if (Recovery.Any(item => item.KeyName.Like(key)) && Recovery.Last(item => item.KeyName.Like(key)) is StackRet ret)
    {
      PurgeStackTo(ret.StackPosition);
    }
    else
    {
      Log(MsgClass.Debug, $"No Catch Defined under {key}.", nameof(Debug));
    }
  }
  #endregion
  /// <summary>Purges the stack until the specified key.</summary>
  /// <param name="key">The key of the position to resume.</param>
  public static void DoCatch (string key) => PurgeStackTo(key);
  /// <summary>Adds a catch point to resume logging in the event of an execption.</summary>
  /// <param name="key">The key to store the position under.</param>
  public static void AddCatch (string key) => Recovery.Add(new(key, LogDepth));
  /// <summary>Sets the logging location for any logs.</summary>
  /// <remarks>This keeps the classname the same as it was since the last <c>DebugIn</c> call.</remarks>
  /// <param name="method">The method name.</param>
  [Obsolete("Avoid this, it has gaps.")]
  public static void DebugIn (string method) => CallStack.Add(new(ThisClass, method));
  /// <summary>Sets the logging location for any logs.</summary>
  /// <param name="classname">The class name.</param>
  /// <param name="method">The method name.</param>
  public static void DebugIn (string classname, string method) => CallStack.Add(new(classname, method));
  /// <summary>Clears the last location set by DebugIn and restores the one before it.</summary>
  public static void DebugOut () => CallStack.Drop();
  /// <summary>Set to define what level of debugging information to display.</summary>
  public static LogClass Verbosity { get; set; }

  /// <summary>Sets the output stream.</summary>
  /// <param name="stream">The stream to output to.</param>
  public static void SetStream (TextWriter stream) => Console.SetOut(stream);
  /// <summary>Clears the console.</summary>
  public static void ClearLog () => Console.Clear();
  /// <summary>Only writes the location of the log, and does not add a newline.</summary>
  /// <param name="cls">The color if not default.</param>
  public static void LogHead (MsgClass cls = MsgClass.Debug) => DoLogHead(cls, ThisClass, ThisMethod);
  /// <summary>Logs a portion of a line to the output stream, and does not append a line feed.</summary>
  /// <param name="cls">The message class of this part.</param>
  /// <param name="part">The text content.</param>
  public static void LogPart (MsgClass cls, string part) =>
    DoLog(part, GetBackColor(cls), GetTextColor(cls), true);
  /// <summary>Writes a newline to the output stream.</summary>
  /// <param name="back">The background color.<br/>Default is black.</param>
  public static void NewLine (ConsoleColor back = C_Black) => DoLog(LFs, back, back, true);
  /// <summary>Logs a message to the output stream using the callers name.</summary>
  /// <remarks>This method always assumes that <see cref="DebugIn(string, string)"/> has been called, and uses that location as the caller.</remarks>
  /// <param name="msgClass">The color and verbosity of the message.</param>
  /// <param name="message">The message text.</param>
  /// <param name="caller">The name of the caller of this method.</param>
  /// <param name="method">The name of this method.</param>
  public static void Log (MsgClass msgClass, string message, string caller, [CallerMemberName] string method = "")
  {
    if (method.IsEmpty)
      DoLogHead(MsgClass.Debug, caller);
    else
      DoLogHead(MsgClass.Debug, caller, method);
    LogPart(msgClass, message);
    NewLine();
  }
  /// <summary>Logs a message to the output stream.</summary>
  /// <remarks>This method always assumes that <see cref="DebugIn(string, string)"/> has been called, and uses that location as the caller.</remarks>
  /// <param name="msgClass">The color and verbosity of the message.</param>
  /// <param name="message">The message text.</param>
  /// <param name="caller">The object calling this method.</param>
  /// <param name="method">The name of this method.</param>
  public static void Log (MsgClass msgClass, string message, object caller, [CallerMemberName] string method = "") => Log(msgClass, message, caller.TypeName, method);
  /// <summary>Logs an exception that was handled internally.</summary>
  /// <param name="e">The exception to log.</param>
  public static void LogException (Exception e) =>
    Log(MsgClass.Error, ThisClass, ThisMethod, e?.Message ?? SE);
}
