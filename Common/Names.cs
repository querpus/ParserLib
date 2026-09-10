#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common;

/// <summary>Shorthand for common enum names.</summary>
public static class Names
{
  /// <summary>Console colors.</summary>
  public const ConsoleColor
    C_Black = ConsoleColor.Black,
    C_Blue = ConsoleColor.Blue,
    C_Cyan = ConsoleColor.Cyan,
    C_DarkRed = ConsoleColor.DarkRed,
    C_DarkYellow = ConsoleColor.DarkYellow,
    C_Green = ConsoleColor.Green,
    C_Red = ConsoleColor.Red,
    C_White = ConsoleColor.White,
    C_Yellow = ConsoleColor.Yellow;

  /// <summary>Creates an instance of the object.</summary>
  public const BindingFlags BFCI = BindingFlags.CreateInstance;
  /// <summary>Specifies the indicated object is <see langword="public"/>.</summary>
  public const BindingFlags BFP = BindingFlags.Public;
  /// <summary>Specifies the indicated object is <see langword="static"/>.</summary>
  public const BindingFlags BFS = BindingFlags.Static;

  /// <summary>No split alterations.</summary>
  public const StringSplitOptions SSON = StringSplitOptions.None;
  /// <summary>Trim and remove empty splits.</summary>
  public const StringSplitOptions SSORT = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

  /// <summary>Ordinal (Case-Sensitive).</summary>
  public const StringComparison SCO = StringComparison.Ordinal;
  /// <summary>Ordinal (Case-Insensitive).</summary>
  public const StringComparison SCOIC = StringComparison.OrdinalIgnoreCase;
  /// <summary>Invariant (Case-Sensitive).</summary>
  public const StringComparison SCI = StringComparison.InvariantCulture;
  /// <summary>Invariant (Case-Insensitive).</summary>
  public const StringComparison SCIIC = StringComparison.InvariantCultureIgnoreCase;

  /// <summary>Number styles.</summary>
  public const NumberStyles
    NSN = NumberStyles.None,
    NSALS = NumberStyles.AllowLeadingSign,
    NSAP = NumberStyles.AllowParentheses,
    NSAT = NumberStyles.AllowThousands,
    NSADP = NumberStyles.AllowDecimalPoint,
    NSABS = NumberStyles.AllowBinarySpecifier,
    NSAHS = NumberStyles.AllowHexSpecifier,
    NSALW = NumberStyles.AllowLeadingWhite;

  /// <summary>No regular expression options.</summary>
  public const RegexOptions RON = RegexOptions.None;
  /// <summary>Dot matches newline.</summary>
  public const RegexOptions ROSL = RegexOptions.Singleline;
  /// <summary>Caret and dollar tokens match beginning and end of lines, not just the string end.</summary>
  public const RegexOptions ROML = RegexOptions.Multiline;
  /// <summary>Case Insensitive.</summary>
  public const RegexOptions ROIC = RegexOptions.IgnoreCase;
  /// <summary>Alternate model, does not catostrphically backtrack.</summary>
  public const RegexOptions RONB = RegexOptions.NonBacktracking;
  /// <summary>Ignore pattern whitespace.</summary>
  public const RegexOptions ROIPW = RegexOptions.IgnorePatternWhitespace;
  /// <summary>Right to left, executes regular expression backwards.</summary>
  public const RegexOptions ROR2L = RegexOptions.RightToLeft;
  /// <summary>Explicit capture, only stores named groups.</summary>
  public const RegexOptions ROEC = RegexOptions.ExplicitCapture;
  /// <summary>Culture Invariant, does not care about cultural differences in language.</summary>
  public const RegexOptions ROCI = RegexOptions.CultureInvariant;

  /// <summary>The current thread's default culture.</summary>
  public static CultureInfo CICC => CultureInfo.CurrentCulture;
  /// <summary>No culture is needed to be specified, does not matter.</summary>
  public static CultureInfo CIIC => CultureInfo.InvariantCulture;
  /// <summary>Culture of the resource manager.</summary>
  public static CultureInfo CICUIC => CultureInfo.CurrentUICulture;
  /// <summary>Culture of the operating System.</summary>
  public static CultureInfo CIIUC => CultureInfo.InstalledUICulture;

  /// <summary>Name for the not found indicator.</summary>
  /// <remarks>Equal to -1.</remarks>
  public const int
    ErrVal = -1,
    NotFound = -1,
    DNE = -1;

  /// <summary>Reference to an empty <see langword="string"/>.</summary>
  /// <remarks>Equal to <see cref="string.Empty"/>.</remarks>
  public static string SE => string.Empty;

  /// <summary>Reference to an empty <see langword="string"/>.</summary>
  /// <remarks>This one is a compile-time constant.</remarks>
  public const string EmptyString = "";
}
