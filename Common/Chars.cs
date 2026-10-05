#pragma warning disable IDE0055 // Formatting

namespace Common;

/// <summary>Static class of common character combinations.</summary>
public static class Chars
{
  #region Characters
  /// <summary>Beep control code.</summary>
  public const char BEEP = '\a';
  /// <summary>Vertical Tab.</summary>
  public const char VT = '\v';
  /// <summary>Horizontal Tab</summary>
  public const char TAB = '\t';
  /// <summary>Carraige Return</summary>
  public const char CR = '\r';
  /// <summary>Line Feed</summary>
  public const char LF = '\n';
  /// <summary>Form Feed</summary>
  public const char FF = '\f';
  /// <summary>Escape</summary>
  public const char ESC = '\e';
  /// <summary>Backspace Character.</summary>
  public const char BSPC = '\b';
  /// <summary>Null Character</summary>
  public const char NUL = '\0';
  /// <summary>Unicode Control Codes.</summary>
  public const char
    SOH = '\u0001',
    STX = '\u0002',
    ETX = '\u0003',
    EOT = '\u0004',
    ENQ = '\u0005',
    ACK = '\u0006',
    SI  = '\u000F',
    ETB = '\u0017',
    CAN = '\u0017',
    SUB = '\u001A',
    FS  = '\u001C',
    GS  = '\u001D',
    RS  = '\u001E',
    US  = '\u001F',
    DEL = '\u007F';
  /// <summary>Single quote symbol.</summary>
  public const char SQ = '\'';
  /// <summary>Double quote symbol.</summary>
  public const char QT = '"';
  #endregion
  #region String Constants
  /// <summary>Standard line ending.</summary>
  public const string CRLF = "\r\n";
  /// <summary>Line feed only.</summary>
  public const string LFs = "\n";
  /// <summary>Carriage return only.</summary>
  public const string CRs = "\r";
  /// <summary>Double quote symbol.</summary>
  public const string QTs = "\"";
  #endregion
  public static readonly ImmutableList<string> NewLines = [CRLF, LFs, CRs, $"{VT}"];
}
