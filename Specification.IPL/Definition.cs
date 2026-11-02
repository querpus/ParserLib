#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Common.Extensions;

using Parser.Inference;
using Parser.Ops.Text;
using Parser.Tokens;

using static Common.Chars;
using static Parser.DefinitionStaticFunctions;
using static Parser.Tokens.TokenRuleType;

namespace Specification.IPL;

[DefinitionExport]
public static class Definition
{
  #region Old Op Data
  /*
  //internal static readonly SplitOperation SplitOp = new(Splits, ROML | ROIPW | ROEC | ROSL, "text", "textparts");
  /// <summary>
  /// <para>Command Splitter Regex</para>
  /// Build from https://regex101.com/r/WJUiK8/2
  /// </summary>
  public static RxSCollection Splits => [
    Rx($@";?{Etx}[\S\s]*?{Stx}"),
    Rx($@"(?<!{Stx})(?=\r|{Cr}|{FS}.*?{FS}|{Can}|{Esc}|{Etb})"),
    $"(^[^><]*{Stx})",
    Etx,
    ";",
    Nul,
    Lf
  ];
  internal static Collection<string> Tokens = [
      "qty",
      "simple",
      "adv",
      "field",
      "d3",
      "standard",
      "fieldtext"];
  */
  private static readonly RxS
    AVal = Gp($",?{Value()}"),
    MVal = AVal.Any,
    Can = Gp($"{CAN}|<CAN>|{CAN.Display}"),
    Etb = Gp($"{ETB}|<ETB>|{ETB.Display}"),
    Eot = Gp($"{EOT}|<EOT>|{EOT.Display}"),
    Esc = Gp($"{ESC}|<ESC>|{ESC.Display}"),
    Ack = Gp($"{ACK}|<ACK>|{ACK.Display}"),
    Etx = Gp($"{ETX}|<ETX>|{ETX.Display}"),
    Stx = Gp($"{STX}|<STX>|{STX.Display}"),
    //Nul = Gp($"{NUL}|<NUL>|{NUL.Display}"),
    //Sub = Gp($"{SUB}|<SUB>"),
    Si = Gp($"{SI}|<SI>|{SI.Display}"),
    Cr = Gp($"{CR}|<CR>|{CR.Display}"),
    Us = Gp($"{US}|<US>|{US.Display}"),
    Rs = Gp($"{RS}|<RS>|{RS.Display}"),
    //Fs = Gp($"{FS}|<FS>"),
    Lf = Gp(@"<LF>|\n"),
    Escape = Nm("escape", Esc),
    Shift = Nm("shift", Si),
    CharPart = Or(ALetter, "<[A-Z]{2,3}>"),
    AnyLazy = Gp(".*?"),
    ALetter = Letter();//,
                       //WS = Gp(@"\s*");

  private static RxS Letter (string c = "[a-zA-Z]") => Nm("t_letter", Gp(c));
  private static RxS Value ([SS("regex")] string c = "[-0-9.]+") => Nm("value", Gp(c));
  private static RxS Cmd (string name, [SS("regex")] string c) => Nm($"m_{name}", $"^{c}$");
  private static RxS Qt ([SS("regex")] string s) => QTs + s + QTs;

  /// <summary>
  /// <para>Command Reader Regex</para>
  /// Built from https://regex101.com/r/dGDxqF/1
  /// </summary>
  public static readonly RxSCollection Regex = [
    Cmd("qty", Or(Us, Rs) + AVal),
    Cmd("simple", Or(Can, Etb, Ack, Cr)),
    Cmd("adv", Eot + CharPart),
    Cmd("field", Escape + Letter("F") + Or(AVal, Qt(Or("[^\"]+", "\\\"")))),
    Cmd("d3", $"{Letter("d")}{Value("3")},{AnyLazy}"),
    Cmd("standard", Escape.Opt + Shift.Opt + ALetter + MVal),
    Cmd("fieldtext", AnyLazy)
  ];
  public static Regex OpRegex => new(Regex, Spec.RxOpt);
  #endregion

  private const RegexOptions RxOpts = ROML | ROIPW | ROEC | ROSL;

  [DefinitionExport]
  public static Spec Spec => new()
  {
    Name = "ipl",
    Operations = [
      new SplitOperation(@"(<ETX>).*?(<STX>)|\A.*?(<STX>)|;", RxOpts, "text", "textparts"),

      //new TokenizeOperation(),
      //new DebugToStringOperation("tokens"),
      //new DebugWaitForInputOperation(),
    ],
    FileInferences = [
      IfN(ExtIs, "ipl"),
      IfN(ExtIs, "pr1"),
      IfN(InferenceType.FileContent|InferenceType.Contains, "<STX>")],
    RxOpt = RxOpts,
    IsTextFile = true,
    TokenType = typeof(string),
    TokenRules = [
      .. TokenRule.MakeSingleCharRules(",;", TokenExact, ["Cm", "Sc"]),
      new(ThrowMatch, "None", $"({Etx}).*?({Stx})"),
      new(ThrowMatch, "None", $@"\A.*?({Stx})"),
      new(ThrowMatch, "None", $@"({Etx})[^<\u0002]*?\z"),
      new(TokenMatch, "Esc", $"{Esc}"),
      new(TokenMatch, "Si", $"{Si}"),
      new(TokenMatch, "Lf", $"{Lf}"),
      new(TokenMatch, "Etb", $"{Etb}"),
      new(TokenMatch, "Rs", $"{Rs}"),
      new(TokenMatch, "Us", $"{Us}"),
      new(TokenMatch, "Can", $"{Can}"),
      new(TokenMatch, "Fs", $"{FS}|<FS>"),
      new(TokenMatch, "Eot", $"{Eot}"),
      new(TokenMatch, "Text", Rx($"(?<=d3,).+?(?=;|{Etx}|{Etb}|{Can}|{Lf})")),
      new(TokenMatch, "OriginX", Rx(@"(?<=o)\d+(?=,)")),
      new(TokenMatch, "OriginY", Rx(@"(?<=o\d+,)\d+(?=;|\<)")),
      new(TokenMatch, "Letter", @"\b[A-Za-z](?=[0-9])"),
      new(TokenMatch, "Letter", @"\b[A-Za-z](?=\< | ;)"),
      new(TokenMatch, "Value", @"(?<=[A-Za-z])[0-9]+(?=,|;|\<)"),
      new(TokenMatch, "Value", @"(?<=,)[0-9]+(?=,|;|\<)"),
    ],
    SC = SCO,
    TokenCompatLookup = new()
    {
      ["Cmd"] = ["Etb", "Can", "Prop", "Ack", "Qty"],
    },
    GroupTokenRules = [
      new("Mode", "x:Esc n:Letter{P} xo:Sc"),
      new("Prop", "n:Letter{o} q:OriginX, x:Cm q:OriginY xo:Sc"),
      new("Prop", "n:Letter{l} q:Value xo:Sc"),
      new("Prop", "n:Letter{w} q:Value xo:Sc"),
      new("FieldNum", "x:Esc n:Letter{F} v:Value xo:Sc"),
      new("Fmt", "n:Letter{E|F} q:Value xo:Sc"),
      new("Line", "n:Letter{[ABDHILMQ]} v:Value qa:Prop xo:Sc"),
      new("Prop", "n:Letter{[chlw]} q:Value xo:Sc"),
      new("Qty", "n:(Rs|Us) q:Value xo:Sc")
    ]
  };
}
