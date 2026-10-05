#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
    Can = Gp($"{CAN}|<CAN>"),
    Etb = Gp($"{ETB}|<ETB>"),
    Eot = Gp($"{EOT}|<EOT>"),
    Esc = Gp($"{ESC}|<ESC>"),
    Ack = Gp($"{ACK}|<ACK>"),
    Etx = Gp($"{ETX}|<ETX>"),
    Stx = Gp($"{STX}|<STX>"),
    //Nul = Gp($"{NUL}|<NUL>"),
    //Sub = Gp($"{SUB}|<SUB>"),
    Si = Gp($"{SI}|<SI>"),
    Cr = Gp($"{CR}|<CR>"),
    Us = Gp($"{US}|<US>"),
    Rs = Gp($"{RS}|<RS>"),
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
    TokenType = typeof(ITT),
    TokenRules = [
      .. TokenRule.MakeSingleCharRules(",;", TokenExact, new ITT[] {ITT.Cm, ITT.Sc}),
      new(ThrowMatch, ITT.None, $"({Etx}).*?({Stx})"),
      new(ThrowMatch, ITT.None, $@"\A.*?({Stx})"),
      new(ThrowMatch, ITT.None, $@"({Etx})[^<\u0002]*?\z"),
      new(TokenMatch, ITT.Esc, $"{Esc}"),
      new(TokenMatch, ITT.Si, $"{Si}"),
      new(TokenMatch, ITT.Lf, $"{Lf}"),
      new(TokenMatch, ITT.Etb, $"{Etb}"),
      new(TokenMatch, ITT.Rs, $"{Rs}"),
      new(TokenMatch, ITT.Us, $"{Us}"),
      new(TokenMatch, ITT.Can, $"{Can}"),
      new(TokenMatch, ITT.Fs, $"{FS}|<FS>"),
      new(TokenMatch, ITT.Eot, $"{Eot}"),
      new(TokenMatch, ITT.SmplCmd, $"(?<={Esc})[CcPT]"),
      new(TokenMatch, ITT.Text, Rx($"(?<=d3,).+?(?=;|{Etx}|{Etb}|{Can}|{Lf})")),
      new(TokenMatch, ITT.OriginX, Rx(@"(?<=o)\d+(?=,)")),
      new(TokenMatch, ITT.OriginY, Rx(@"(?<=o\d+,)\d+(?=;|\<)")),
      new(TokenMatch, ITT.Letter, @"\b[A-Za-z](?=[0-9])"),
      new(TokenMatch, ITT.Letter, @"\b[A-Za-z](?=\< | ;)"),
      new(TokenMatch, ITT.Value, @"(?<=[A-Za-z])[0-9]+(?=,|;|\<)"),
      new(TokenMatch, ITT.Value, @"(?<=,)[0-9]+(?=,|;|\<)"),
    ],
    SC = SCO,
    TokenCompatLookup = new()
    {
      [ITT.Cmd] = [ITT.Etb, ITT.Can, ITT.Prop, ITT.Ack, ITT.Qty],
    },
    GroupTokenRules = [
      new(ITT.Mode, "x:Esc n:SmplCmd{P} xo:Sc"),
      new(ITT.Prop, "n:Letter{o} q:OriginX, x:Cm q:OriginY xo:Sc"),
      new(ITT.Prop, "n:Letter{l} q:Value xo:Sc"),
      new(ITT.Prop, "n:Letter{w} q:Value xo:Sc"),
      new(ITT.FieldNum, "x:Esc n:Letter{F} v:Value xo:Sc"),
      new(ITT.Fmt, "n:Letter{E|F} q:Value xo:Sc"),
      new(ITT.Line, "n:Letter{[ABDHILMQ]} v:Value qa:Prop xo:Sc"),
      new(ITT.Prop, "n:Letter{[chlw]} q:Value xo:Sc"),
      new(ITT.Qty, "n:(Rs|Us) q:Value xo:Sc")
    ]
  };
}
