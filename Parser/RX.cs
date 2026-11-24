using static Common.RegExp.RegexStaticFunctions;

namespace Parser;

public static partial class RX
{
  #region Precompiled Regex
  // Pregenerated Regex
  public const string LnEnd = @"(?:\r\n?|\n)";
  [GeneratedRegex(LnEnd)]
  public static partial Regex LineEnd { get; }

  public const string GpName = @"(?:(?<=\(\?\<)(?<groupname>\w+?)(?=\>\)))";
  [GeneratedRegex(GpName)]
  public static partial Regex GroupName { get; }

  public const string XlRngSplit = @"\s*,(?![^{}]*\})\s*";
  [GeneratedRegex(XlRngSplit)]
  public static partial Regex XlRangeSplitter { get; }
  public const string TokenTemplateDef = @"^(?'ref_name'\w+)\s*\-\s*(?'line''(?'literal'.+?)'|(?'ws_req'\ )|(?'opt_ws_or_comment'\-)|\#(?'ref'\w+)|(?'gp_start'\()|(?'gp_end'\))|(?'opt'\?)|(?'more'\+)|(?'or'\|))*$\s*";
  [GeneratedRegex(TokenTemplateDef)]
  public static partial Regex TokenTemplateDefinition { get; }
  #endregion

  // Named Groups
  public static RxS G_CLnComment => field = Nm("lncomment", CLnComment);
  public static RxS G_CBlkComment => field = Nm("blkcomment", CBlkComment);
  public static RxS G_CPreProc => field = Nm("preproc", CPreProc);
  public static RxS G_WS => field = Nm("ws", WS);
  public static RxS G_Name => field = Nm("name", Name);
  public static RxS G_CString => field = Nm("string", CString);
  public static RxS G_Int => field = Nm("int", IntegerRx);

  // C-Style Common Parts
  public static RxS CLnComment => field = Gp(@"\/\/[^\n\r]*");
  public static RxS CBlkComment => field = Gp(@"\/\*[\s\S]*?\*\/");
  public static RxS CPreProc => field = Rx(@"^\s*\#.+?$");
  public static RxS CString => field = Rx(@"(?n:""([^\\""]|\\.)*"")");

  // Other Common Parts
  public static RxS Chars => field = $"'{Or(@"[^\\]", @"\\[\\abefnr0]", @"\\u\d\d\d\d")}'";
  public static RxS Name => field = Rx(@"[a-zA-Z_][\w]*");
  public static RxS WS => field = Rx(@"\s+");
  public static RxS WSO => field = Rx(@"\s*");
  public static RxS IntegerRx => field = Rx(@"-?\d+");
  public static RxS DecimalRx => field = Or(@"-?\d*\.\d+", IntegerRx);
  public static RxS Boolean => field = Rx(@"(?n:\b(false|true)\b)");

  // Modifiers
  public static RxS CaseInsensitive { get; } = Rx("(?i)");
  public static RxS CaseSensitive { get; } = Rx("(?-i)");
  public static RxS Extended { get; } = Rx("(?x)");
  public static RxS NotExtended { get; } = Rx("(?-x)");
}
