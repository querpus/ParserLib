#pragma warning disable CA1710 // Identifiers should have correct suffix

using Common.Nodes;

using MC = Common.MsgClass;

namespace Parser.Tokens;

/// <summary>Match Tokenizer</summary>
/// <remarks>Each match is a token.</remarks>
public sealed class TokenFactory : ITokenizer<IToken>
{
  IEnumerable<IToken> ITokenizer<IToken>.Tokenize (string input) => Tokenize(input);
  IEnumerable<IToken> ITokenizer<IToken>.Tokens => Tokens;
  #region Private Fields
  private const string Area = "TokenFactory";
  private readonly TokenRuleCollection _rules = [];
  private readonly TokenCollection _result = [];
  private TokenRule? _currentRule;
  private Spec _spec;
  private int _ruleIndex;
  private int _ruleIndexAfterCompete;
  #endregion
  #region Public Properties
  public Spec Spec { get => _spec; init => _spec = value; }
  public string Input { get; private set; } = SE;
  [NotNull]
  public SectionCollection? CannotMatch { get; private set; }
  public bool PromptAfterEach { get; set; }
  public Collection<IToken> Tokens => [.. _result];
  #endregion
  #region Constructors
  public TokenFactory (Spec spec, IEnumerable<TokenRule>? rules = null)
  {
    SetSpec(spec);
    _rules.AddRange(rules ?? spec.TokenRules);
  }
  public TokenFactory () => SetSpec(XParser.Lib["unknown"]);
  #endregion
  #region Private Helper Properties
  private RT DefaultRule => _spec.DefaultRuleSet;
  private bool IgnoreCase => (_currentRule?.Type.HasFlag(RT.IgnoreCase) ?? false) || DefaultRule.HasFlag(RT.IgnoreCase);
  private StringComparison IC => IgnoreCase ? SCOIC : SCO;
  private bool Competes => (_currentRule?.Type.HasFlag(RT.Competitive) ?? false) || DefaultRule.HasFlag(RT.Competitive);
  private bool IgnoredToken => (_currentRule?.Type.HasFlag(RT.IgnoredToken) ?? false) || DefaultRule.HasFlag(RT.IgnoredToken);
  private bool HasError => _currentRule?.Type.HasFlag(RT.Error) ?? false;
  private RT Type => GetMaskedType(_currentRule?.Type ?? RT.None);
  private string RuleData => _currentRule?.RuleStringData ?? SE;
  private Action RuleAction => Type switch
  {
    RT when HasError => () => ErrorLog("Error: Invalid rule. Skipping rule."),
    RT when Competes => RunCompete,
    RT.None => () => WarnLog("Warning: No type defined. Skipping rule."),
    RT.TokenMatch or RT.TokenExtract or RT.SplitMatch => RegexMatch,
    RT.TokenExact or RT.SplitExact => ExactMatch,
    RT.StoreExtra => StoreExtra,
    RT.StoreOther => StoreOther,
    RT.ErrorMatch => ErrorMatch,
    _ => () => WarnLog("Warning: Bad type defined. Skipping rule.")
  };

  dynamic? ITokenizer<IToken>.Rules { get => _spec; init => _spec = (value as Spec) ?? throw new InvalidTokenException("Attempt to assign a null Spec."); }

  #endregion
  #region Private Logging Methods
  private static void DebugLog (string msg) => Log(MC.Debug, msg, nameof(TokenFactory));
  private static void WarnLog (string msg) => Log(MC.Warning, msg, nameof(TokenFactory));
  private static void ErrorLog (string msg) => Log(MC.Error, msg, nameof(TokenFactory));
  #endregion
  #region Private Static Methods
  private static string GetRuleRegex (TokenRule rule, int? index = null)
  {
    string regex = rule?.RuleStringData ?? SE;

    if (GetMaskedType(rule?.Type ?? RT.None) == RT.TokenExact)
    {
      regex = Regex.Escape(regex);
    }

    string casemod = rule?.Type.HasFlag(RT.IgnoreCase) ?? false ? "(?i)" : "(?-i)";

    return index is not null ? $"{casemod}(?'_R{index}'{regex})" : $"{casemod}{regex}";
  }
  private static RT GetMaskedType (RT type) => type.RemoveBitLong<RT>(RT.FlagBits);
  private static int GetRuleGroupIndex (Match match)
  {
    DebugIn(Area, "GetRuleGroupIndex");
    string num = match.Groups.
      AsReadOnly().
      First(static g => g.Name.StartsWith("_R", SCO) && g.Value.Length > 0).
      Name[2..];
    int result = int.TryParse(num, out int value) ? value : ErrVal;
    if (result == ErrVal)
    {
      ErrorLog("GetRuleGroupIndex Returned -1");
    }
    DebugOut();
    return result;
  }
  #endregion
  private void MakeAddToken (Pos match, TokenRule? rule = null)
  {
    rule ??= _currentRule!;

    if (rule.Type.HasFlag(RT.IgnoredToken))
      return;

    bool singleChar = false;
    if (match.Start == match.End)
    {
      singleChar = true;
    }

    Token token = new()
    {
      Index = match.Start,
      Content = singleChar ? $"{Input[match.Start]}" : Input[match.Start..(match.End + 1)],
      Type = rule.TypeToAssign,
      Spec = _spec
    };

    _result.Add(token);
  }
  private void StoreOther ()
  {
    DebugIn(Area, "StoreOther");
    DebugLog("Storing remaining zones.");
    foreach (Pos applicant in CannotMatch.Inverse())
    {
      DebugLog($"Section: {applicant} Found with no token.");
      CannotMatch.Add(applicant.Start, applicant.Length);
      MakeAddToken(applicant);
    }
    DebugOut();
  }
  /// <summary>Checks every unmatched section for a match, and if said section matches, adds the token.</summary>
  /// <remarks>This will NOT see anything but the unmatched section, so any lookaheads or lookbehinds will FAIL.</remarks>
  private void StoreExtra ()
  {
    DebugIn(Area, "StoreExtra");
    DebugLog($"Storing remaining zones matching {RuleData}");
    foreach (Pos applicant in CannotMatch.Inverse())
    {
      string text = CannotMatch.GetText(applicant);
      if (Regex.IsMatch(text, RuleData))
      {
        foreach (Match m in Regex.Matches(text, RuleData))
        {
          Pos tsec = new(applicant.Start + m.Index, m.Length);
          CannotMatch.Add(tsec);
          MakeAddToken(tsec);
        }
      }
    }
    DebugOut();
  }
  /// <summary>Exact (no regex) match, uses the rule's ignore case property, not the spec's one.</summary>
  private void ExactMatch ()
  {
    DebugIn(Area, "ExactMatch");
    DebugLog("Token Exact starting, from input string.");
    int length = 0;
    if (RuleData.Length > 0)
      length = RuleData.Length;
    else
      _ = Err.ThrowBadDef("RuleData has a length of 0 on an exact token.");

    int cursor = 0;
    int next = Input.IndexOf(RuleData, cursor, IC);

    while (next >= 0 && cursor <= Input.Length)
    {
      Section match = new(next, length, Input);
      if (!CannotMatch.Overlaps(match))
      {
        if (Type is RT.TokenExact)
        {
          MakeAddToken(match);
          CannotMatch.Add(match);
        }
      }
      cursor = next + 1;
      next = Input.IndexOf(RuleData, cursor, IC);
    }
    DebugOut();
  }
  private void RegexMatch ()
  {
    DebugIn(Area, "RegexMatch");
    DebugLog("Token matching starting, from input string.");
    Regex regex = new(RuleData, IgnoreCase ? _spec.RxOpt | ROIC : _spec.RxOpt);

    MatchCollection mc = regex.Matches(Input);

    foreach (Match match in mc)
    {
      Pos rng = new(match.Index, match.Length);

      if (!CannotMatch.Overlaps(rng))
      {
        if (Type is RT.TokenExtract)
        {
          foreach (Pos c in match.Groups["keep"].Captures.Select(c => new Pos(c.Index, c.Length)))
            MakeAddToken(c);
        }
        else if (Type is RT.TokenMatch)
        {
          MakeAddToken(rng);
        }

        CannotMatch.Add(rng);
      }
    }
    DebugOut();
  }
  private void ErrorMatch ()
  {
    DebugIn(Area, "ErrorMatch");
    DebugLog("Error matching starting, from input string.");
    Regex regex = new(RuleData, IgnoreCase ? _spec.RxOpt | ROIC : _spec.RxOpt);

    MatchCollection mc = regex.Matches(Input);
    bool failUponEnding = false;
    foreach (Match match in mc)
    {
      failUponEnding = true;

      int error_pos = match.Groups.ContainsKey("error_pos") ? match.Groups["error_pos"].Index : match.Index;
      int error_len = match.Groups.ContainsKey("error_pos") ? match.Groups["error_pos"].Length : match.Index;
      int error_surround_pos = match.Groups.ContainsKey("error_surround") ? match.Groups["error_surround"].Index : -1;
      int error_surround_len = match.Groups.ContainsKey("error_surround") ? match.Groups["error_surround"].Length : -1;
      (int err_line, int err_col) = Input.Get2DPosition(error_pos);
      string[] lines = Input.Split('\n');
      int line_max = lines.Length;
      WarnLog($"Error at line {err_line}, column {err_col}.");
      ErrorPlacement err = new() { Error = match, Text = Input };
      err.WriteError();
    }
    if (failUponEnding)
      _ = Err.ThrowBadResult("You must correct the above listed errors to parse this file.");
    DebugOut();
  }
  private IEnumerable<(TokenRule Rule, int Index)> GetCompeteBlock ()
  {
    Collection<(TokenRule Rule, int Index)> result = [];
    for (int i = _ruleIndex; i < _rules.Count; i++)
    {
      if (_rules[i].Type.HasFlag(RT.Competitive) && _rules[i].RuleStringData is not null)
      {
        result.Add((_rules[i], i));
      }
      else
      {
        _ruleIndexAfterCompete = i;
        return result;
      }
    }
    _ruleIndexAfterCompete = _rules.Count;
    return result;
  }
  private void RunCompete ()
  {
    DebugIn(Area, "RunCompete");
    DebugLog("Running competition.");
    IEnumerable<(TokenRule Rule, int Index)> contestants = GetCompeteBlock();
    string regexPatterns = contestants.Select(r => GetRuleRegex(r.Rule, r.Index)).TextJoin("|");
    Regex regex = new(regexPatterns, _spec.RxOpt.RemoveBit<RegexOptions>(ROIC));

    foreach (Match match in regex.Matches(Input))
    {
      int index = GetRuleGroupIndex(match);
      Pos rng = new(match.Index, match.Length);

      _currentRule = _rules[index];

      if (!IgnoredToken)
        MakeAddToken(rng);

      CannotMatch.Add(rng);
    }
    _ruleIndex = _ruleIndexAfterCompete > _ruleIndex ? _ruleIndexAfterCompete : _ruleIndex;
    DebugOut();
  }
  #region Public Methods
  [MemberNotNull(nameof(_spec))]
  public void SetSpec (Spec spec) => _spec = spec;
  [MemberNotNull(nameof(CannotMatch))]
  public Collection<IToken> Tokenize (string input)
  {
    DebugIn(Area, "Produce");
    _result.Clear();
    CannotMatch = new(input);
    input.ThrowIfNull();
    Input = input;
    for (_ruleIndex = 0; _ruleIndex < _rules.Count; _ruleIndex++)
    {
      TokenRule rule = _rules[_ruleIndex];
      _currentRule = rule;

      try
      {
        RuleAction.Invoke();
      }
      catch (OperationException e)
      {
        LogException(e); throw new OperationException("Error found.", e);
      }
      _result.SortByIndex();
    }
    DebugOut();
    return [.. _result];
  }
  #endregion
}
