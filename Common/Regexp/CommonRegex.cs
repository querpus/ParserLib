//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1056

using static Common.RegExp.RegexStaticFunctions;

namespace Common.RegExp;

/// <summary>Common Regex Parts.</summary>
public static class CommonRegex
{
  // Common Patterns
  public static string WS_NoLine { get; } = Gp(@"[^\S\r\n]+");
  public static string WS_OnlyLine { get; } = Gp(@"(?:\r\n?|\n)+");
  public static string WS_OnlyNonLF { get; } = Gp(@"(?:\r\n?)+");
  public static string WS_Mult { get; } = Gp(@"\s{2,}");
  /// <summary>Required Whitespace (including new lines)</summary>
  public static string WS_Req { get; } = Gp(@"\s+");
  /// <summary>Optional Whitespace (including new lines)</summary>
  public static string WS { get; } = Gp(@"\s*");

  public static string NWS_OrSlash { get; } = Gp(@"[^\s\/]+");
  public static string NWS_OrSemiC { get; } = Gp(@"[^\s;]+");

  public static string SQuote { get; } = Gp("'.*?'");
  public static string AQuote { get; } = Gp(@"(?<qt>[""']).*?\k<qt>");
  public static string DQuote { get; } = Gp(@""".*?""");
  public static string NQuote { get; } = Gp(@"\<.*?\>");
  public static string BQuote { get; } = Gp(@"\[.*?\]");
  public static string CQuote { get; } = Gp(@"\{.*?\}");
  public static string PQuote { get; } = Gp(@"\(.*?\)");

  public static string URL_Name { get; } = Gp(@"(?:[\w-]|\%[a-fA-F0-9]{2})+");
  public static string URL_Domain { get; } = Nm("fulldomain", Gp(Nm("subdomain", URL_Name) + @"\.") + "*" + Nm("domain", URL_Name) + @"\." + Nm("ext", URL_Name));
  public static string URL_Query { get; } = Nm("fullquery", $@"(?:\?(?<query>(?<var>\w+)\=(?<value>{URL_Vars})(?:\&(?<var>\w+)\=(?<value>{URL_Vars}))*))");
  public static string URL_Vars { get; } = Gp(@"(?:[\w+.-]|\%[a-fA-F0-9]{2})+");
  public static string URL_IPv4 { get; } = Nm("ip", @"\d+\.\d+\.\d+\.\d+");
  public static string URL { get; } = Nm("url", Nm("protocol", URL_Name) + @$"\:\/\/(?:{URL_IPv4}|{URL_Domain}(?:\:{Nm("port", @"\d+")})?(?<path>{Gp(URL_Name + @"\/")}*{URL_Name})?" + Gp(@"\." + Nm("fileext", URL_Name)) + "?" + URL_Query);

  //Pre-Built Regex Replace Nodes
  public static ReplaceNode LineCommonizer { get; } = new(WS_OnlyNonLF, "\n");
  public static ReplaceNode WhitespaceReducer { get; } = new(WS_Mult, " ");
  public static ReplaceNode WhitespaceReducer_NoLines { get; } = new(WS_NoLine, " ");

  public static string INIStyle_CommentIgnores { get; } = Nm("valid", $"(?:{AQuote}|{NWS_OrSemiC})+");
  public static string INIStyle_CommentReduces { get; } = Nm("reduce", @"(?:\s+|;.*$)+");

  public static string CStyle_NoCommentStart { get; } = Gp(@"\/(?![\*\/])");
  public static string CStyle_CommentIgnores { get; } = Nm("valid", $"(?:{AQuote}|{NWS_OrSlash}|{CStyle_NoCommentStart})+");
  public static string CStyle_CommentReduces { get; } = Nm("reduce", @"(?:\s+|\/\/.*$|\/\*[\s\S]*?\*\/)+");
}
